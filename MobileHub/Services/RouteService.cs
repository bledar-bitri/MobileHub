using System;
using RouteModel;
using DataAccessLayer.Managers.Route;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Contracts;
using Logging.Interfaces;
using RoutePlanner;
using TspWithTimeWindows;
using Utilities;

namespace Services
{
    public class RouteService : IRouteService
    {
        private readonly SemaphoreSlim _syncLock = new SemaphoreSlim(20);

        private readonly RoadInfoDataManager _roadInfoManager = new RoadInfoDataManager();

        private static int webLookupCount;
        private int asyncId = 1;

        public RoadInfo GetRoadInfo(int fromLatitude, int fromLongitude, int toLatitude, int toLongitude)
        {
            return _roadInfoManager.GetRoadInfo(fromLatitude, toLatitude, fromLongitude, toLongitude);
        }

        public List<RoadInfo> GetAllRoadInfos()
        {
            throw new NotImplementedException();
        }

        private readonly IAddressService addressService;

        public RouteService(IAddressService addressService)
        {
            this.addressService = addressService;
        }


        #region Road Distance

        public List<RoadInfoContract> LoadDistances(List<AddressContract> addresses, ILogger logger, int userId)
        {
            var list = new List<RoadInfoContract>();
            foreach (var address in addresses)
            {
                webLookupCount = 0;
                LoadDistances(address, addresses, list, logger, userId);
                Trace.TraceInformation($"From: [{address.Latitude} {address.Longitude}] Weblookup: {webLookupCount}");
            }
            return list;

        }

        public async void LoadDistances(AddressContract currentAddress, List<AddressContract> addresses, List<RoadInfoContract> listToLoad, ILogger logger, int userId)
        {
            if (currentAddress?.Latitude == null || !currentAddress.Longitude.HasValue) return;
            var currentAddressText =
                $"{currentAddress?.Street}, {currentAddress?.Zip}, {currentAddress?.City}, {currentAddress?.Country}";
            var tasks = new List<Task<RoadInfoContract>>();

            foreach (var address in addresses)
            {
                if (currentAddress.Id == address.Id) continue;

                if (address.Latitude.HasValue && address.Longitude.HasValue)
                    tasks.Add(
                        GetDistance(
                            currentAddress.Id,
                            currentAddressText,
                            GeoCodeConverter.ToGeoCoordinate(currentAddress.Latitude.Value),
                            GeoCodeConverter.ToGeoCoordinate(currentAddress.Longitude.Value),
                            address.Id,
                            $"{address.Street}, {address.Zip}, {address.City}, {address.Country}",
                            GeoCodeConverter.ToGeoCoordinate(address.Latitude.Value),
                            GeoCodeConverter.ToGeoCoordinate(address.Longitude.Value), 
                            logger, 
                            userId
                        ));
            }
            Trace.TraceInformation("\n\n\n\n WAITING FOR ALL TASKS TO FINISH \n\n\n\n");
            await Task.WhenAll(tasks);
            Trace.TraceInformation("\n\n\n\n ALL TASKS DONE \n\n\n\n");
            listToLoad.AddRange(tasks.Select(task => task.Result));
        }
        public async Task<RoadInfoContract> GetDistance(int fromAddressId, string fromAddress, double fromLatitude, double fromLongitude, int toAddressId, string toAddress, double toLatitude, double toLongitude, ILogger logger, int userId)
        {
            try
            {
                return await GetDistance(fromAddressId, fromAddress, fromLatitude, fromLongitude, toAddressId, toAddress, toLatitude, toLongitude, 3);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, userId, "GRRR: " + ex.Message);
            }
            return null;
        }

        private async Task<RoadInfoContract> GetDistance(int fromAddressId, string fromAddress, double fromLatitude, double fromLongitude, int toAddressId, string toAddress, double toLatitude, double toLongitude, int tries)
        {

            var roadDistance = _roadInfoManager.GetRoadInfo(
                GeoCodeConverter.ToInteger(fromLatitude),
                GeoCodeConverter.ToInteger(fromLongitude),
                GeoCodeConverter.ToInteger(toLatitude),
                GeoCodeConverter.ToInteger(toLongitude),
                true);
            
            if (roadDistance == null)
            {
                return await GetGeocodeDistance(fromAddressId, fromAddress, fromLatitude, fromLongitude, toAddressId, toAddress, toLatitude, toLongitude, tries);
            }
            // Lookup distance online if older than 6 months (maybe roads changed...)
            if (roadDistance.LookupDate == null || DateTime.Now.Subtract(roadDistance.LookupDate.Value).TotalDays > 180)
            {
                return await GetGeocodeDistance(fromAddressId, fromAddress, fromLatitude, fromLongitude, toAddressId, toAddress, toLatitude, toLongitude, tries, true);
            }

            return new RoadInfoContract(fromAddressId, toAddressId, roadDistance); 
        }
        
        private async Task<RoadInfoContract> GetGeocodeDistance(int fromAddressId, string fromAddress, double fromLatitude, double fromLongitude, int toAddressId, string toAddress, double toLatitude, double toLongitude, int tries, bool updateDistance = false)
        {
            var query = $"wp.0={fromLatitude},{fromLongitude}&wp.1={toLatitude},{toLongitude}";
            var geocodeRequest = new Uri($"http://dev.virtualearth.net/REST/v1/Routes/Driving?{query}&key={CommonConfigValues.BingMapsKey}");
            var road = new RoadInfo
            {
                FromAddress = fromAddress,
                FromLatitude = GeoCodeConverter.ToInteger(fromLatitude),
                FromLongitude = GeoCodeConverter.ToInteger(fromLongitude),
                ToAddress = toAddress,
                ToLatitude = GeoCodeConverter.ToInteger(toLatitude),
                ToLongitude = GeoCodeConverter.ToInteger(toLongitude),
                LookupDate = DateTime.UtcNow
            };
            
//            GetResponse(geocodeRequest, road, asyncId++, SetGeocodeDistanceFromResponse);
            await GetResponseAsync(geocodeRequest, road, asyncId++);
            return new RoadInfoContract(fromAddressId, toAddressId, road);
        }


        private void SetGeocodeDistanceFromResponse(Response response, RoadInfo road, int counter)
        {
            Trace.TraceInformation($" Response from: {counter} \n");
            if (response.ResourceSets[0].Resources.Length > 0)
            {
                foreach (var resource in response.ResourceSets[0].Resources)
                {
                    var res = resource as Route;
                    if (res == null) continue;
                    // save road info
                    using (var manager = new RoadInfoDataManager()) // need to reopen the database context because of the async nature of the call
                    {
                        
                        webLookupCount++;
                        string stats;
                        road.Distance = res.TravelDistance;
                        road.TimeInSeconds = (long) res.TravelDuration;
                        manager.SaveRoadInfo(road, out stats);
                        
                    }
                    //Trace.TraceInformation($"From: [{road.FromLatitude} {road.FromLongitude}] --> [{road.ToLatitude} {road.ToLongitude}] Distance: {road.Distance}");
                }
            }
        }
        
        private async Task<RoadInfo> GetResponseAsync(Uri uri, RoadInfo road, int counter)
        {
            var wc = new WebClient();
            _syncLock.Wait();
            var result = await wc.OpenReadTaskAsync(uri);
            var ser = new DataContractJsonSerializer(typeof(Response));

            SetGeocodeDistanceFromResponse(ser.ReadObject(result) as Response, road, counter);
            _syncLock.Release();
            return road;
        }
        #endregion

        #region Generate Route

        public List<CityContract> CalculateRouteForUserId(int userId, ILogger logger)
        {
            var addressContracs = new List<AddressContract>();
            var roadInfoContracs = new List<RoadInfoContract>();
            
            logger?.LogMessage(userId, "Loading Addresses");


            var addresses = addressService.GetUserAddresses(1);
            addressService.LoadGeocodeInformation(addresses[0]);

            logger?.LogMessage(userId, "Address Contracts");
            int count = 0;
            addresses.ForEach(a =>
            {
                if (a.Latitude.HasValue && a.Longitude.HasValue)
                {
                    addressContracs.Add(new AddressContract(count++, a));
                }
            });

            roadInfoContracs = LoadDistances(addressContracs, logger, userId);

            var rp = new RoutePlanerManager(addressContracs, roadInfoContracs, true, logger, userId);
            rp.Run();
            var result = new List<CityContract>();
            rp.BestTour.ForEach(tourCity => result.Add(new CityContract{Id = tourCity.Id, Name = tourCity.Name}));
            return result;
        }
        #endregion

        #region IDisposable


        public void Dispose()
        {
            _roadInfoManager.Dispose();
        }


        #endregion



    }
}

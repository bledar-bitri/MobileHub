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
using Utilities;

namespace Services
{
    public class RouteService : IRouteService, IDisposable
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

        public void Dispose()
        {
            _roadInfoManager.Dispose();
        }

        #region Road Distance

        public List<RoadInfo> LoadDistances(List<AddressContract> addresses)
        {
            var list = new List<RoadInfo>();
            foreach (var address in addresses)
            {
                webLookupCount = 0;
                LoadDistances(address, addresses, list);
                Trace.TraceInformation($"From: [{address.Latitude} {address.Longitude}] Weblookup: {webLookupCount}");
            }
            return list;

        }

        public async void LoadDistances(AddressContract currentAddress, List<AddressContract> addresses, List<RoadInfo> listToLoad)
        {
            if (currentAddress?.Latitude == null || !currentAddress.Longitude.HasValue) return;

            var tasks = new List<Task<RoadInfo>>();

            foreach (var address in addresses)
            {
                if (currentAddress.Id == address.Id) continue;

                if (address.Latitude.HasValue && address.Longitude.HasValue)
                    tasks.Add(
                        GetDistance(GeoCodeConverter.ToGeoCoordinate(currentAddress.Latitude.Value),
                        GeoCodeConverter.ToGeoCoordinate(currentAddress.Longitude.Value),
                        GeoCodeConverter.ToGeoCoordinate(address.Latitude.Value),
                        GeoCodeConverter.ToGeoCoordinate(address.Longitude.Value)
                        ));
            }
            Trace.TraceInformation("\n\n\n\n WAITING FOR ALL TASKS TO FINISH \n\n\n\n");
            await Task.WhenAll(tasks);
            Trace.TraceInformation("\n\n\n\n ALL TASKS DONE \n\n\n\n");
            listToLoad.AddRange(tasks.Select(task => task.Result));
        }
        public async Task<RoadInfo> GetDistance(double fromLatitude, double fromLongitude, double toLatitude, double toLongitude)
        {
            try
            {
                return await GetDistance(fromLatitude, fromLongitude, toLatitude, toLongitude, 3);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GRRR: " + ex.Message);
            }
            return null;
        }

        private async Task<RoadInfo> GetDistance(double fromLatitude, double fromLongitude, double toLatitude, double toLongitude, int tries)
        {

            var roadDistance = _roadInfoManager.GetRoadInfo(
                GeoCodeConverter.ToInteger(fromLatitude),
                GeoCodeConverter.ToInteger(fromLongitude),
                GeoCodeConverter.ToInteger(toLatitude),
                GeoCodeConverter.ToInteger(toLongitude),
                true);

            if (roadDistance == null)
            {
                return await GetGeocodeDistance(fromLatitude, fromLongitude, toLatitude, toLongitude, tries);
            }
            // Lookup distance online if older than 6 months (maybe roads changed...)
            if (roadDistance.LookupDate == null || DateTime.Now.Subtract(roadDistance.LookupDate.Value).TotalDays > 180)
            {
                return await GetGeocodeDistance(fromLatitude, fromLongitude, toLatitude, toLongitude, tries, true);
            }

            return roadDistance;            
        }

        private async Task<RoadInfo> ConvertToRoadInfoTask(RoadInfo ri)
        {
            return ri;
        }
        private async Task<RoadInfo> GetGeocodeDistance(double fromLatitude, double fromLongitude, double toLatitude, double toLongitude, int tries, bool updateDistance = false)
        {
            var query = $"wp.0={fromLatitude},{fromLongitude}&wp.1={toLatitude},{toLongitude}";
            var geocodeRequest = new Uri($"http://dev.virtualearth.net/REST/v1/Routes/Driving?{query}&key={CommonConfigValues.BingMapsKey}");
            var road = new RoadInfo
            {
                FromLatitude = GeoCodeConverter.ToInteger(fromLatitude),
                FromLongitude = GeoCodeConverter.ToInteger(fromLongitude),
                ToLatitude = GeoCodeConverter.ToInteger(toLatitude),
                ToLongitude = GeoCodeConverter.ToInteger(toLongitude),
                LookupDate = DateTime.UtcNow
            };

//            GetResponse(geocodeRequest, road, asyncId++, SetGeocodeDistanceFromResponse);
            await GetResponseAsync(geocodeRequest, road, asyncId++);
            return road;
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
        

        private void GetResponse(Uri uri, RoadInfo road, int counter, Action<Response, RoadInfo, int> callback)
        {
            var wc = new WebClient();
            wc.OpenReadCompleted += (o, a) =>
            {
                if (callback != null)
                {
                    Trace.TraceInformation($"From: [{road.FromLatitude} {road.FromLongitude}] --> [{road.ToLatitude} {road.ToLongitude}] Distance: {road.Distance}");
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Response));
                    callback(ser.ReadObject(a.Result) as Response, road, counter);
                    _syncLock.Release();
                }
            };
            Trace.TraceInformation($" STARTING: {counter} ");
            _syncLock.Wait();
            wc.OpenReadAsync(uri);
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

        #endregion




    }
}

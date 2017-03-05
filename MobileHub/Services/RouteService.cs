using System;
using RouteModel;
using DataAccessLayer.Managers.Route;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using System.Threading;
using Common;
using Contracts;
using Utilities;

namespace Services
{
    public class RouteService : IRouteService, IDisposable
    {
        private readonly RoadInfoDataManager _roadInfoManager = new RoadInfoDataManager();
        private static readonly Object SynchLocker = new Object();

        private static int webLookupCount;

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

        public void LoadDistances(List<AddressContract> addresses)
        {
            foreach (var address in addresses)
            {
                webLookupCount = 0;
                LoadDistances(address, addresses);
                Trace.TraceInformation($"From: [{address.Latitude} {address.Longitude}] Weblookup: {webLookupCount}");
            }
        }

        public void LoadDistances(AddressContract currentAddress, List<AddressContract> addresses)
        {
            if (currentAddress?.Latitude == null || !currentAddress.Longitude.HasValue) return;

            foreach (var address in addresses)
            {
                if (currentAddress.Id == address.Id) continue;

                if (address.Latitude.HasValue && address.Longitude.HasValue)
                    LoadDistances(GeoCodeConverter.ToGeoCoordinate(currentAddress.Latitude.Value),
                        GeoCodeConverter.ToGeoCoordinate(currentAddress.Longitude.Value),
                        GeoCodeConverter.ToGeoCoordinate(address.Latitude.Value),
                        GeoCodeConverter.ToGeoCoordinate(address.Longitude.Value)
                        );
            }
        }
        public void LoadDistances(double fromLatitude, double fromLongitude, double toLatitude, double toLongitude)
        {
            try
            {
                SetDistance(fromLatitude, fromLongitude, toLatitude, toLongitude, 3);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GRRR: " + ex.Message);
            }
        }

        private void SetDistance(double fromLatitude, double fromLongitude, double toLatitude, double toLongitude, int tries)
        {

            var roadDistance = _roadInfoManager.GetRoadInfo(
                GeoCodeConverter.ToInteger(fromLatitude),
                GeoCodeConverter.ToInteger(fromLongitude),
                GeoCodeConverter.ToInteger(toLatitude),
                GeoCodeConverter.ToInteger(toLongitude),
                true);

            if (roadDistance == null)
            {
                SetGeocodeDistance(fromLatitude, fromLongitude, toLatitude, toLongitude, tries);
            }
            // Lookup distance online if older than 6 months (maybe roads changed...)
            else if (roadDistance.LookupDate == null || DateTime.Now.Subtract(roadDistance.LookupDate.Value).TotalDays > 180)
            {
                SetGeocodeDistance(fromLatitude, fromLongitude, toLatitude, toLongitude, tries, true);
            }
            else
            {
                /*
                if (!Stop)
                {
                    road.Distance = roadDistance.Distance;
                    road.TravelTimeInSeconds = roadDistance.TimeInSeconds;
                }
                */
            }
        }

        private void SetGeocodeDistance(double fromLatitude, double fromLongitude, double toLatitude, double toLongitude, int tries, bool updateDistance = false)
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

            GetResponse(geocodeRequest, road, SetGeocodeDistanceFromResponse);
        }


        private void SetGeocodeDistanceFromResponse(Response response, RoadInfo road)
        {
            if (response.ResourceSets[0].Resources.Length > 0)
            {
                foreach (var resource in response.ResourceSets[0].Resources)
                {
                    var res = resource as Route;
                    if (res == null) continue;
                    // save road info
                    using (var manager = new RoadInfoDataManager()) // need to reopen the database context because of the async nature of the call
                    {
                        lock (SynchLocker)
                        {
                            webLookupCount++;
                            string stats;
                            road.Distance = res.TravelDistance;
                            road.TimeInSeconds = (long) res.TravelDuration;
                            manager.SaveRoadInfo(road, out stats);
                        }
                    }
                    //Trace.TraceInformation($"From: [{road.FromLatitude} {road.FromLongitude}] --> [{road.ToLatitude} {road.ToLongitude}] Distance: {road.Distance}");
                }
            }
        }

        private void GetResponse(Uri uri, RoadInfo road, Action<Response, RoadInfo> callback)
        {
            var wc = new WebClient();
            wc.OpenReadCompleted += (o, a) =>
            {
                if (callback != null)
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Response));
                    callback(ser.ReadObject(a.Result) as Response, road);
                }
            };
            wc.OpenReadAsync(uri);
        }
        #endregion
        
    }
}

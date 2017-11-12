using System;
using System.Collections.Generic;
using DataAccessLayer.Managers;
using Google.Api.Maps.Service.Geocoding;
using Google.Api.Maps.Service;
using Contracts;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Threading;
using Utilities;
using Common;
using CustomerModel;

namespace Services
{
    public class AddressService : BaseService, IAddressService, IDisposable
    {
        private readonly SemaphoreSlim _syncLock = new SemaphoreSlim(20);
        private readonly AddressDataManager _manager = new AddressDataManager();
        
        public List<CustomerModel.Address> GetAllAddresses()
        {
            var addresses = _manager.GetAllAddresses();
            return addresses;
        }

        public CustomerModel.Address GetAddress(string id)
        {
            return _manager.GetAddress(id);
        }
        public List<CustomerModel.Address> GetUserAddresses(int userId)
        {
            return _manager.GetUserAddresses(userId);
        }
        public void SaveAddresses(List<CustomerModel.Address> addresses)
        {
            string stats;
            _manager.SaveAddress(addresses, out stats);
        }

        public void SaveAddress(CustomerModel.Address address)
        {
            string stats;
            _manager.SaveAddress(new List<CustomerModel.Address> { address }, out stats);
        }


        public void Dispose()
        {
            _manager.Dispose();
        }

        #region Address Coordinates
        public void LoadGeocodeInformation(CustomerModel.Address address)
        {
            LoadGeocodeCoordinatesFromWeb(address);
        }

        /*
         * Try Bing First and then GoogleMaps
         */
        private void LoadGeocodeCoordinatesFromWeb(CustomerModel.Address address)
        {
            var query = $"{address.Street}, {address.Zip}, {address.City}, {address.Country.Name}";
            var geocodeRequest = new Uri($"http://dev.virtualearth.net/REST/v1/Locations?q={query}&key={CommonConfigValues.BingMapsKey}");

            GetResponse(geocodeRequest, address, GetGeocodeLocationFromResponse);
        }

        private void GetGeocodeLocationFromResponse(Response response, CustomerModel.Address address)
        {
            if (response.ResourceSets[0].Resources.Length > 0)
            {
                foreach (var resource in response.ResourceSets[0].Resources)
                {
                    var fromResponse = resource as Location;
                    if (fromResponse == null) continue;

                    var locationFromResponse = fromResponse;
                    address.Latitude = GeoCodeConverter.ToInteger(locationFromResponse.Point.Coordinates[0]);
                    address.Longitude = GeoCodeConverter.ToInteger(locationFromResponse.Point.Coordinates[1]);
                    
                    using (var manager = new AddressDataManager()) // need to reopen the database context because of the async nature of the call
                    {
                        string stats;
                        manager.SaveAddress(new List<CustomerModel.Address> {address}, out stats);
                    }
                    Console.WriteLine("{0} result(s) found for Address {1}", response.ResourceSets[0].Resources.Length, locationFromResponse.Name);
                    Console.WriteLine("\t\t [{0}] [{1}]", locationFromResponse.Point.Coordinates[0], locationFromResponse.Point.Coordinates[1]);
                }
            }
        }

        private void GetResponse(Uri uri, CustomerModel.Address address, Action<Response, CustomerModel.Address> callback)
        {
            WebClient wc = new WebClient();
            wc.OpenReadCompleted += (o, a) =>
            {
                if (callback != null)
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Response));
                    callback(ser.ReadObject(a.Result) as Response, address);
                    _syncLock.Release();
                }
            };
            _syncLock.Wait();
            wc.OpenReadAsync(uri);
        }

        private GeocodeLocation[] LoadGeocodeAddressFromGoogleMaps(string address)
        {

            var request = new GeocodingRequest
            {
                Address = address,
                Sensor = "false"
            };
            var response = GeocodingService.GetResponse(request);
            if (response.Status == ServiceResponseStatus.Ok)
            {
                if (response.Results == null || response.Results.Length <= 0)
                {
                    // bad address ... do something with it
                    return null;
                }
                else
                {
                    var location = new GeocodeLocation
                    {
                        Latitude = Convert.ToDouble(response.Results[0].Geometry.Location.Latitude),
                        Longitude = Convert.ToDouble(response.Results[0].Geometry.Location.Longitude)
                    };
                    return new GeocodeLocation[] { location };
                }
            }
            else
            {
                // bad address ... do something with it
                return null;
            }
        }

       
        #endregion
    }
}

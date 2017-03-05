using System;
using System.Collections.Generic;
using Contracts;
using RouteModel;
using Services;

namespace TestServices
{
    public class TestRouteService
    {
        public TestRouteService()
        {
            Console.WriteLine("================ Testing The Route Service ================");
            var addressContracs = new List<AddressContract>();

            using (var service = new AddressService())
            {
                Console.WriteLine("Loading Addresses");


                var addresses = service.GetAllAddresses();
                //service.LoadGeocodeInformation(addresses[0]);
                /*
                addresses.ForEach(a =>
                {
                    service.LoadGeocodeInformation(a);

                });//*/
                Console.WriteLine("Address Contracts");
                addresses.ForEach(a =>
                {
                    if (a.Latitude.HasValue && a.Longitude.HasValue)
                    {
                        addressContracs.Add(new AddressContract(a));
                    }
                });
            }

            using (var service = new RouteService())
            {
                Console.WriteLine("Loading Distances");
                service.LoadDistances(addressContracs);
            }
            Console.WriteLine("================ DONE ================");
        }
    }
}

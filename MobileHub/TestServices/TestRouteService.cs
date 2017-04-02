using System;
using System.Collections.Generic;
using Contracts;
using RouteModel;
using RoutePlanner;
using Services;

namespace TestServices
{
    public class TestRouteService
    {
        public TestRouteService()
        {
            Console.WriteLine("================ Testing The Route Service ================");
            var addressContracs = new List<AddressContract>();
            var roadInfoContracs = new List<RoadInfoContract>();

            using (var service = new AddressService())
            {
                Console.WriteLine("Loading Addresses");


                var addresses = service.GetUserAddresses(1);
                service.LoadGeocodeInformation(addresses[0]);
                /*
                addresses.ForEach(a =>
                {
                    service.LoadGeocodeInformation(a);

                });*/
                Console.WriteLine("Address Contracts");
                int count = 0;
                addresses.ForEach(a =>
                {
                    if (a.Latitude.HasValue && a.Longitude.HasValue)
                    {
                        addressContracs.Add(new AddressContract(count++, a));    
                    }
                });
            }

            using (var service = new RouteService())
            {
                Console.WriteLine("Loading Distances");
                roadInfoContracs = service.LoadDistances(addressContracs);
                Console.WriteLine($"{roadInfoContracs.Count} Distances Loaded");
            }

            new RoutePlanerManager(addressContracs, roadInfoContracs, true).Run();
            Console.WriteLine("================ DONE ================");
        }
    }
}

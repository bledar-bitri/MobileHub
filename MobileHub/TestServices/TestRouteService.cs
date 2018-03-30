using System;
using System.Collections.Generic;
using Contracts;
using Logging;
using Logging.Interfaces;
using Ninject;
using AutoMapper;
using RoutePlanner;
using Services;

namespace TestServices
{
    public class TestRouteService
    {
        private StandardKernel _standardKernel;

        public TestRouteService(StandardKernel kernel)
        {
            _standardKernel = kernel;
            AutoMapperConfig.Configure();
        }
        public void Test() { 

            Console.WriteLine("================ Testing The Route Service ================");
            var clientId = Guid.NewGuid().ToString();
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
                    if (!a.Latitude.HasValue || !a.Longitude.HasValue) return;
                    var addressContract = Mapper.Map<CustomerModel.Address, AddressContract>(a);
                    addressContract.Id = count++;
                    addressContracs.Add(addressContract);
                });
            }

            using (var service = _standardKernel.Get<IRouteService>())
            {
                Console.WriteLine("Loading Distances");
                var bestTour = service.CalculateRouteForUserId(clientId, 1, _standardKernel.Get<ILogger>());
                Console.WriteLine($"{roadInfoContracs.Count} Distances Loaded");
            }

            new RoutePlanerManager(clientId, addressContracs, roadInfoContracs, true, _standardKernel.Get<ILogger>(), 1).Run();
            Console.WriteLine("================ DONE ================");
        }


        public void TestRouteGenerationForUserId(string clientId, int userId)
        {
            using (var service = _standardKernel.Get<IRouteService>())
            {
                var bestTour = service.CalculateRouteForUserId(clientId, 1, _standardKernel.Get<ILogger>());
            }
            Console.WriteLine("================ DONE ================");
        }
    }
}

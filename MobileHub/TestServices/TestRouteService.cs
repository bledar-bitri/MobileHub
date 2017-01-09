using Services;

namespace TestServices
{
    public class TestRouteService
    {
        public TestRouteService()
        {

            using (var service = new AddressService())
            {
                var addresses = service.GetAllAddresses();
                service.LoadGeocodeInformation(addresses[0]);
                /*
                addresses.ForEach(a =>
                {
                    service.LoadGeocodeInformation(a);

                });*/
            }
        }
    }
}

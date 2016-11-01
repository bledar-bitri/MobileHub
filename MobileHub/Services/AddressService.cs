using System;
using System.Collections.Generic;
using CustomerModel;
using DataAccessLayer.Managers;

namespace Services
{
    public class AddressService : IAddressService, IDisposable
    {
        private readonly AddressDataManager _manager = new AddressDataManager();


        public List<Address> GetAllAddresses()
        {
            var addresses = _manager.GetAllAddresses();
            return addresses;
        }

        public Address GetAddress(string id)
        {
            return _manager.GetAddress(id);
        }

        public void Dispose()
        {
            _manager.Dispose();
        }
    }
}

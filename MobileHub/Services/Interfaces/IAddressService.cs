using System.Collections.Generic;
using CustomerModel;

namespace Services
{
    public interface IAddressService
    {
        #region Data Access

        List<Address> GetAllAddresses();
        Address GetAddress(string id);

        void SaveAddresses(List<Address> addresses);
        void SaveAddress(Address address);
        #endregion

        void LoadGeocodeInformation(Address address);
    }
}

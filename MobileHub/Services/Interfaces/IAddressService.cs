using System.Collections.Generic;
using CustomerModel;

namespace Services
{
    public interface IAddressService
    {
        #region Data Access

        List<Address> GetAllAddresses();
        Address GetAddress(string id);


        #endregion

    }
}

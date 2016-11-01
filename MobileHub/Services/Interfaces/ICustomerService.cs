using System.Collections.Generic;
using Contracts;
using CustomerModel;

namespace Services
{
    public interface ICustomerService
    {
        #region Data Access


        List<Customer> GetAllCustomers();
        Customer GetCustomer(string id);

        List<CustomerContract> GetAllCustomerContracts();


        #endregion

    }
}

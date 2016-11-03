using CustomerModel;

namespace Contracts
{
    using System.Collections.Generic;
    
    public partial class CustomerContract
    {
        public string CompanyId { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string AddressId { get; set; }

        public int AccountManagersUserId { get; set; }

        public string OriginalCustomerId { get; set; }

        public string CustomerTypeId { get; set; }

        public CustomerContract() { }

        public CustomerContract(Customer customer)
        {
            CompanyId = customer.CompanyId;
            FirstName = customer.FirstName;
            LastName = customer.LastName;
            AddressId = customer.AddressId;
            AccountManagersUserId = customer.AccountManagersUserId;
            OriginalCustomerId = customer.OriginalCustomerId;
            CustomerTypeId = customer.CustomerTypeId;
        }
    }
}
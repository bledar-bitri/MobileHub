using System;
using System.Collections.Generic;
using Contracts;
using CustomerModel;
using DataAccessLayer.Managers;

namespace Services
{
    public class CustomerService : BaseService, ICustomerService, IDisposable
    {
        private readonly CustomerDataManager _manager = new CustomerDataManager();


        public List<Customer> GetAllCustomers()
        {
            var customers = _manager.GetAllCustomers();
            return customers;
        }

        public Customer GetCustomer(string id)
        {
            return _manager.GetCustomer(id);
        }

        public List<CustomerContract> GetAllCustomerContracts()
        {
            var customers = _manager.GetAllCustomers();
            var contracts = new List<CustomerContract>();
            
            customers.ForEach(c => contracts.Add(AutoMapper.Mapper.Map<Customer, CustomerContract>(c)));
            
            return contracts;
        }

        public void Dispose()
        {
            _manager.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions.Impl;
using CustomerModel;

namespace Contracts
{
    public class AddressContract
    {
        public AddressContract()
        {
            
        }

        public AddressContract(CustomerModel.Address address)
        {
            Assign(address);
        }

        private void Assign(CustomerModel.Address address)
        {
            if(address == null) return;

            Id = address.Id;
            Street = address.Street;
            Street2 = address.Street2;
            City = address.City;
            Sate = address.Sate;
            Zip = address.Zip;
            CountryId = address.CountryId;
            Latitude = address.Latitude;
            Longitude = address.Longitude;
        }

        public string Id { get; set; }

        public string Street { get; set; }

        public string Street2 { get; set; }

        public string City { get; set; }

        public string Sate { get; set; }

        public string Zip { get; set; }

        public string CountryId { get; set; }

        public long? Latitude { get; set; }

        public long? Longitude { get; set; }
    }
}

namespace Contracts
{
    public class AddressContract
    {
        
        public int Id { get; set; }

        public string AddressId { get; set; }

        public string Street { get; set; }

        public string Street2 { get; set; }

        public string City { get; set; }

        public string Sate { get; set; }

        public string Zip { get; set; }

        public string CountryId { get; set; }

        public long? Latitude { get; set; }

        public long? Longitude { get; set; }

        public string Country { get; set; }

        private string _address;

        public string Address => _address;

        public AddressContract(int id)
        {
            Id = id;
        }

        public AddressContract(int id, CustomerModel.Address address)
        {
            Id = id;
            Assign(address);
        }

        private void Assign(CustomerModel.Address address)
        {
            if(address == null) return;

            AddressId = address.Id;
            Street = address.Street;
            Street2 = address.Street2;
            City = address.City;
            Sate = address.Sate;
            Zip = address.Zip;
            CountryId = address.CountryId;
            Latitude = address.Latitude;
            Longitude = address.Longitude;
            Country = address.Country?.Name;
            _address = $"{Street},{Zip},{City},{Country}";
        }

    }
}

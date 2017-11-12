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

        public string Address => $"{Street},{Zip},{City},{Country}";
    }
}

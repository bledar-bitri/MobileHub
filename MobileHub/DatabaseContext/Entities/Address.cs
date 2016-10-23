using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseContext.Entities
{
    public class Address : Entity
    {
        public string Street { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public int CountryID { get; set; }
        public long Latitude { get; set; }
        public long Longitude { get; set; }

        public virtual Country Country { get; set; }
    }
}

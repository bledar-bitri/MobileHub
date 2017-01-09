using Contracts;
using System;
using System.Diagnostics;
using System.Linq;

namespace TspWithTimeWindows
{
    [DebuggerDisplay("AddressLocation {DebuggerDisplay}")]
    [Serializable]
    public class AddressLocation
    {
        public AddressWithID Address { get; set; }
        public GeocodeLocation[] Locations { get; set; }

        public AddressLocation() { }
        public AddressLocation(AddressWithID address, GeocodeLocation[] locations)
        {
            Address = address;
            Locations = locations;
        }
        public bool Equals(AddressLocation address)
        {
            return address.Address == Address;
        }
        public string DebuggerDisplay
        {
            get { return string.Format("{0} Locations [{1}]", Address.DebuggerDisplay, String.Join("; ", Locations.Select(l => l.Latitude + ", " + l.Longitude))); }
        }

        public string GetDumpData()
        {
            return DebuggerDisplay;
        }
    }
}

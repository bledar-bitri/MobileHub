using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;

namespace TspWithTimeWindows
{
    [DebuggerDisplay("AddressWithID {DebuggerDisplay}")]
    [XmlRoot(ElementName = "AddressWithID", IsNullable = false)]
    [Serializable]
    public class AddressWithID
    {
        public int ID { get; set; }
        public string Address { get; set; }
        public List<int> OtherIds { get; set; }
        public List<AddressLocation> SuggestedAddresses { get; set; }

        public AddressWithID()
        {
        }

        public AddressWithID(int id, string address)
        {
            ID = id;
            Address = address;
            OtherIds = new List<int>();
            SuggestedAddresses = new List<AddressLocation>();
        }

        public bool Equals(AddressWithID address)
        {
            //            return ID == address.ID && Address == address.Address;
            return Address == address.Address;
        }

        public string DebuggerDisplay
        {
            get { return string.Format("[ADDR: {0}] [ID: {1}]", Address, ID); }
        }
    }
}

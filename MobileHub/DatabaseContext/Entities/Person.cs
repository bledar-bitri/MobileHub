using System;
using System.Collections.Generic;

namespace DatabaseContext.Entities
{
    public class Person : Entity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public string EMailAddress { get; set; }
        public int AddressID { get; set; }
        public virtual Address Address { get; set; }
        public virtual ICollection<Meeting> Meetings { get; set; }

    }
}

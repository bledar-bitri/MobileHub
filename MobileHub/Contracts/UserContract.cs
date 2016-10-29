

using System;
using System.Collections.Generic;
using CustomerModel;
using SecurityModel;

namespace Contracts
{
    public class UserContract 
    {
        public int ID { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public string EMailAddress { get; set; }
        public Address Address { get; set; }
        public ICollection<Meeting> Meetings { get; set; }

        public UserContract()
        {
            
        }
        public UserContract(User user)
        {
            ID = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
        }

        public User ToEntity()
        {
            var user = new User
            {
                Id = ID,
                FirstName = FirstName,
                LastName = LastName,
            };
            return user;
        }
    }
}

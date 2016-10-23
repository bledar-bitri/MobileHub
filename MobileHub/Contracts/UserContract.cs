

using System;
using System.Collections.Generic;
using DatabaseContext.Entities;

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
            DateOfBirth = user.DateOfBirth;
            EMailAddress = user.EMailAddress;
            Address = user.Address;

            if (user.Meetings == null) return;

            foreach (var meeting in user.Meetings)
            {
                Meetings.Add(meeting);
            }
        }

        public User ToEntity()
        {
            var user = new User
            {
                Id = ID,
                FirstName = FirstName,
                LastName = LastName,
                DateOfBirth = DateOfBirth,
                EMailAddress = EMailAddress,
                Address = Address,
            };
            if (Meetings != null)
            {
                foreach (var meeting in Meetings)
                {
                    user.Meetings.Add(meeting);
                }
            }
            return user;
        }
    }
}

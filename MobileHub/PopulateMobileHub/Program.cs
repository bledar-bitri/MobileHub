using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using DatabaseContext;
using DatabaseContext.Entities;

namespace PopulateMobileHub
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*****************************************************");
            Console.WriteLine("Populating Mobile Hub With Data");
            Console.WriteLine("*****************************************************\n\n");


            using (var ctx = new MobileHubContext())
            {
                #region countries

                var countries = new List<Country>
                {
                    new Country
                    {
                        Abbreviation = "DE",
                        Name = "Germany"
                    },
                    new Country
                    {
                        Abbreviation = "AT",
                        Name = "Austria"
                    },
                };

                #endregion

                #region addresses

                var addresses = new List<Address>
                {
                    new Address
                    {
                        Street = "slavi soucek str 11",
                        City = "Salzburg",
                        Country = countries[1],
                        Zip = "5026"
                    },

                    new Address
                    {
                        Street = "rennbahnstrasse 4b",
                        City = "Salzburg",
                        Country = countries[1],
                        Zip = "5020"
                    }

                };

                #endregion

                
                #region users

                var brad = new User
                {
                    FirstName = "Brad",
                    LastName = "Pit",
                    UserName = "b.pit",
                    DateOfBirth = new DateTime(1977, 10, 13),
                    EMailAddress = "bledi1@yahoo.com",
                    LastLogonTime = DateTime.Now,
                    Address = addresses[0],

                };
                var george = new User
                {
                    FirstName = "George",
                    LastName = "Clooney",
                    UserName = "g.clooney",
                    DateOfBirth = new DateTime(1977, 10, 13),
                    EMailAddress = "bledi1@yahoo.com",
                    LastLogonTime = DateTime.Now,
                    Address = addresses[1],
                };

                var users = new List<User>
                {
                    brad,
                    george
                };
                #endregion

                #region customers

                var mj = new Customer
                {
                    FirstName = "Michael",
                    LastName = "Jordan",
                    DateOfBirth = new DateTime(1977, 10, 13),
                    EMailAddress = "mj@yahoo.com",
                    Address = addresses[0],
                    User = brad
                };

                var lebron = new Customer
                {
                    FirstName = "Lebron",
                    LastName = "James",
                    DateOfBirth = new DateTime(1977, 10, 13),
                    EMailAddress = "lj@yahoo.com",
                    Address = addresses[0],
                    User = george
                };

                var customers = new List<Customer>
                {

                    mj, lebron
                };

                #endregion
                #region meetings

                var bradMjMeeting1 = new Meeting
                {
                    AddressID = 1,
                    MeetingTime = DateTime.Now.AddDays(-1),
                    Purpose = "First Meeting",
                    Memo = "First Meeting Between Brad and MJ",
                    User = brad,
                    Customer = mj
                };
                var bradMjMeeting2 = new Meeting
                {
                    AddressID = 1,
                    MeetingTime = DateTime.Now,
                    Purpose = "Second Meeting",
                    Memo = "Second Meeting Between Brad and MJ",
                    User = brad,
                    Customer = mj
                };

                var georgeLebronMeeting1 = new Meeting
                {
                    AddressID = 1,
                    MeetingTime = DateTime.Now.AddDays(-1),
                    Purpose = "Get To know each other",
                    Memo = "First Impressions between George and Lebron",
                    User = george,
                    Customer = lebron
                };

                var georgeLebronMeeting2 = new Meeting
                {
                    AddressID = 1,
                    MeetingTime = DateTime.Now,
                    Purpose = "Second Meeting",
                    Memo = "Second Meeting Between George and Lebron",
                    User = george,
                    Customer = lebron
                };

                
                var meetings = new List<Meeting>
                {
                    bradMjMeeting1,
                    bradMjMeeting2,
                    georgeLebronMeeting1,
                    georgeLebronMeeting2
                };
                #endregion


                Console.WriteLine("Adding data to the context...\n");
                countries.ForEach(c => ctx.Countries.Add(c));
                addresses.ForEach(a => ctx.Addresses.Add(a));
                customers.ForEach(c => ctx.Customers.Add(c));
                users.ForEach(u => ctx.Users.Add(u));
                meetings.ForEach(m => ctx.Meetings.Add(m));
                Console.WriteLine("\nSaving Changes ...\n");
                ctx.SaveChanges();
                Console.WriteLine("\nDONE!");
                Thread.Sleep(500);
            }
        }
    }
}

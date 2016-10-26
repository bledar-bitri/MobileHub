using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using LoginDatabaseContext;

namespace PopulateMobileHub
{
    public class CustomerDatabasePopulator
    {
        static Random rnd = new Random();

        public static void Populate(List<User> users)
        {
            Console.WriteLine("*****************************************************");
            Console.WriteLine("Populating Customer Database With Data");
            Console.WriteLine("*****************************************************\n\n");
            

            using (var ctx = new MobileHubCustomerContext())
            {

                #region locales

                var locales = new List<Locale>
                {
                    new Locale()
                    {
                        Name = "German - Germany",
                        Code = "de",
                        LcidString = "de-de",
                        LcidDecimal = 1031,
                        LcidHex = 407,
                        LcidCodePage = 1252
                    },
                    new Locale()
                    {
                        Name = "English - United States",
                        Code = "en",
                        LcidString = "en-us",
                        LcidDecimal = 1033,
                        LcidHex = 409,
                        LcidCodePage = 1252
                    },

                };
                #endregion

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
                
                #region customer users
                
                var customerUsers = new List<CustomerUser>();

                users.ForEach(u =>
                {
                    int r = rnd.Next(locales.Count);
                    customerUsers.Add(
                        new CustomerUser
                        {
                            Id = u.Id,
                            Locale = locales[r]
                        });
                });

                #endregion

                #region companiess

                var company1 = new CustomerCompany
                {
                    Name = "test company",
                    Address = addresses[0]
                };
                var company2 = new CustomerCompany
                {
                    Name = "second test company",
                    Address = addresses[1]
                };

                var companies = new List<CustomerCompany> {company1, company2};
                #endregion

                #region customer types

                var customerTypes = new List<CustomerType>
                {
                    new CustomerType
                    {
                        Name = "Normal"
                    }
                };
                #endregion

                #region customers

                var mj = new Customer
                {
                    FirstName = "Michael",
                    LastName = "Jordan",
                    Address = addresses[0],
                    CustomerCompany = company1,
                    AccountManagersUserId = users[0].Id
                };

                var lebron = new Customer
                {
                    FirstName = "Lebron",
                    LastName = "James",
                    Address = addresses[0],
                    CustomerCompany = company2,
                    AccountManagersUserId = users[1].Id
                };

                var customers = new List<Customer>
                {

                    mj, lebron
                };

                #endregion

                #region action types

                var actionTypes = new List<ActionType>
                {
                    new ActionType
                    {
                        Code = 1,
                        Description = "Normal Action"
                    },
                    new ActionType
                    {
                        Code = -1,
                        Description = "Invisible Action"
                    },
                };
                #endregion

                #region available actions

                var availableActions = new List<AvailableAction>
                {
                    new AvailableAction
                    {
                        ActionCode = 1,
                        ActionType = actionTypes[0],
                        CustomerType = customerTypes[0],
                        Locale = locales[0],
                        Name = "Nicht Getroffen",
                        Description = "Kunde nicht getrofen"
                    },
                    new AvailableAction
                    {
                        ActionCode = 1,
                        ActionType = actionTypes[0],
                        CustomerType = customerTypes[0],
                        Locale = locales[1],
                        Name = "Not Found",
                        Description = "Customer Not Found"
                    }
                };
                #endregion

                #region meetings

                var meetings = new List<Meeting>();

                foreach (var user in customerUsers)
                {
                    var meeting1 = new Meeting
                    {
                        Address = addresses[0],
                        MeetingTime = DateTime.Now.AddDays(-1),
                        Purpose = "First Meeting",
                        Memo = "First Meeting Between Brad and MJ",
                        CustomerUser = user,
                        Customer = mj
                    };
                    var meeting2 = new Meeting
                    {
                        Address = addresses[1],
                        MeetingTime = DateTime.Now,
                        Purpose = "Second Meeting",
                        Memo = "Second Meeting Between Brad and MJ",
                        CustomerUser = user,
                        Customer = mj
                    };
                    meetings.Add(meeting1);
                    meetings.Add(meeting2);
                }

                #endregion

                #region action history

                var actionHistory = new List<ActionHistory>
                {
                    new ActionHistory
                    {
                        Address = addresses[0],
                        Customer = customers[0],
                        CustomerUser = customerUsers[0],
                        ActionCode = availableActions[0].ActionCode,
                        ActionTime = DateTime.Now,
                        Memo = "some crappy memo"
                    }
                };
                #endregion

                Console.WriteLine("Adding data to the context...\n");
                locales.ForEach(l => ctx.Locales.Add(l));
                countries.ForEach(c => ctx.Countries.Add(c));
                addresses.ForEach(a => ctx.Addresses.Add(a));
                customerUsers.ForEach(u => ctx.CustomerUsers.Add(u));
                companies.ForEach(c => ctx.CustomerCompanies.Add(c));
                customerTypes.ForEach(c => ctx.CustomerTypes.Add(c));
                customers.ForEach(c => ctx.Customers.Add(c));
                actionTypes.ForEach(a => ctx.ActionTypes.Add(a));
                availableActions.ForEach(a => ctx.AvailableActions.Add(a));
                meetings.ForEach(m => ctx.Meetings.Add(m));
                actionHistory.ForEach(a => ctx.ActionHistories.Add(a));
                Console.WriteLine("\nSaving Changes ...\n");
                ctx.SaveChanges();
                Console.WriteLine("\nDONE!");
            }
        }
    }
}

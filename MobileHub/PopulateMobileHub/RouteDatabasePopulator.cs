using System;
using System.Collections.Generic;
using PopulateMobileHub.htb;
using System.Linq;
using System.Data.Entity.Infrastructure;
using CustomerModel;
using System.Data.Entity;

namespace PopulateMobileHub
{
    public class RouteDatabasePopulator
    {

        public void PopulateCustomerTableBasedOnAddresses()
        {

            Console.WriteLine("*****************************************************");
            Console.WriteLine("Populating Customer Table Data");
            Console.WriteLine("*****************************************************\n\n");

            var rnd = new Random();
            
            using (var ctx = new MobileHubCustomerContext())
            {
                var addresses = (from a in Query<Address>(ctx) select a).Take(50).ToList();
                var companies = (from a in Query<CustomerCompany>(ctx) select a).ToList();
                var users = (from a in Query<CustomerUser>(ctx) select a).ToList();
                var customerTypes = (from a in Query<CustomerType>(ctx) select a).ToList();
                var idx = 1;

                foreach (var address in addresses)
                {

                    var companiesIdx = rnd.Next(companies.Count);
                    var usersIdx = rnd.Next(users.Count);
                    var customerTypeIdx = rnd.Next(customerTypes.Count);

                    var customer = new Customer
                    {
                        Id = Guid.NewGuid().ToString(),
                        CreatedAt = DateTime.UtcNow,
                        FirstName = $"FirstName_{idx}",
                        LastName = $"LastName_{idx}",
                        AddressId = address.Id,
                        CompanyId = companies[companiesIdx].Id,
                        AccountManagersUserId = users[usersIdx].Id,
                        CustomerTypeId = customerTypes[customerTypeIdx].Id
                    };
                    ctx.Customers.Add(customer);
                    idx++;
                }
                ctx.SaveChanges();
            }

        }

        public void PopulateAddressTable()
        {
            Console.WriteLine("*****************************************************");
            Console.WriteLine("Populating Address Table Data");
            Console.WriteLine("*****************************************************\n\n");
            List<tblGegnerAdressen> htbAddresses;

            using (var ctx = new HTBContext())
            {
                Console.WriteLine("Reading data from HTB...\n");
                var query = from g in Query<tblGegnerAdressen>(ctx) where g.GAZipPrefix == "A" select g;
                htbAddresses = query.ToList();
            }
            if (!htbAddresses.Any()) return;

            using (var ctx = new MobileHubCustomerContext())
            {

                Console.WriteLine("Adding data to the context...\n");
                var countryQuery = from c in Query<Country>(ctx) where c.Abbreviation == "AT" select c;
                var country = countryQuery.FirstOrDefault();
                if (country == null)
                {
                    Console.WriteLine("Cound not find country with abbreviation 'AT'...\n");
                    return;
                }
                for (var i = 0; i < 10000; i++)
                {
                    var a = htbAddresses[i];
                    if (string.IsNullOrEmpty(a.GAStrasse) || string.IsNullOrEmpty(a.GAOrt) ||
                        string.IsNullOrEmpty(a.GAZIP)) continue;

                    if (
                        ctx.Addresses.FirstOrDefault(
                            address =>
                                address.Street == a.GAStrasse && address.City == a.GAOrt && address.Zip == a.GAZIP &&
                                address.CountryId == country.Id) != null) continue;
                    try
                    {
                        ctx.Addresses.Add(
                            new Address
                            {
                                Id = Guid.NewGuid().ToString(),
                                Street = a.GAStrasse,
                                City = a.GAOrt,
                                Sate = a.GAOrt,
                                Zip = a.GAZIP,
                                CountryId = country.Id,
                                CreatedAt = DateTime.UtcNow
                            });
                        Console.WriteLine("Adding {0}, {1}, {2}", a.GAStrasse, a.GAZIP, a.GAOrt);
                        ctx.SaveChanges();

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error occurred while saving Address Information {0}", e.Message);
                    }
                }
                Console.WriteLine("\nDONE!");
            }

        }

        /// <summary>
        /// Basic query with NoTracking-Option and optional Includes, which are passed as a list of strings
        /// </summary>
        protected IQueryable<TEntity> Query<TEntity>(DbContext ctx, bool? tracking = null, List<string> includes = null)
         where TEntity : class
        {
            var q = GetBaseQuery<TEntity>(ctx, tracking);
            if (includes != null) // Eager Loading?
            {
                foreach (var include in includes.Where(x => !string.IsNullOrEmpty(x)))
                {
                    q = q.Include(include);
                }
            }
            return q;
        }

        /// <summary>
        /// Internal helper method that provides dbquery with or without tracking
        /// </summary>
        private DbQuery<TEntity> GetBaseQuery<TEntity>(DbContext ctx, bool? tracking) where TEntity : class
        {
            DbQuery<TEntity> q = ctx.Set<TEntity>();
            if ((tracking.HasValue && tracking.Value == false) || (tracking.HasValue == false)) // Tracking nicht gewünscht
            {
                q = q.AsNoTracking();
            }
            return q;
        }
    }
}

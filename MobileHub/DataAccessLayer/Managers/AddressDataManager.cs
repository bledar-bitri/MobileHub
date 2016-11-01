using System.Data.Entity;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using CustomerModel;
using DataAccessLayer.HelperClasses;


namespace DataAccessLayer.Managers
{
    /// <summary>
    /// Datamanager for Customer entities
    /// </summary>
    public class AddressDataManager : DataManagerBase<MobileHubCustomerContext>
    {
        
        /// <summary>
        /// Contructor used by the business logic
        /// </summary>
        public AddressDataManager(bool preloading = false, bool lazyLoadingDefault = false, bool tracking = false)
         : this(preloading, lazyLoadingDefault, tracking, null)
        {

        }

        /// <summary>
        /// This constructor must be internal otherwise we need a EF Reference in the busines logic
        /// </summary>
        internal AddressDataManager(bool preloading = true, bool lazyLoadingDefault = false, bool tracking = false, MobileHubCustomerContext customerContext = null)
        : base(lazyLoadingDefault, tracking, customerContext)
        {
            if (preloading) ctx.Customers.ToList();
        }

        public Address GetAddress(string customerId, bool? tracking = null)
        {
            var query = from c in Query<Address>(tracking) where c.Id == customerId select c;
            return query.SingleOrDefault();
        }

        public List<Address> GetAllAddresses()
        {
            return GetAddresses(null);
        }


        public List<Address> GetAddresses(string street)
        {
            IQueryable<Address> query = ctx.Addresses;
            if (!string.IsNullOrEmpty(street))
                query = query.Where(p => p.Street.Contains(street) || p.Street.Contains(street));

            query = query.Include(a => a.Country);

            return query.ToList();
        }

        public List<Address> GetCustomerByLatitudeAndLongitude(long latitude, long longituge)
        {

            return ctx.Addresses.Where(a => a.Latitude == latitude && a.Longitude == longituge).ToList();
        }
        
        public List<Address> SaveAddress(List<Address> address, out string statistics)
        {
            return Save(address, out statistics);
        }
    }
}

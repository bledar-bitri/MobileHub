using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using CustomerModel;
using DataAccessLayer.HelperClasses;


namespace DataAccessLayer.Managers
{
    /// <summary>
    /// Datamanager for Customer entities
    /// </summary>
    public class CustomerDataManager : DataManagerBase<MobileHubCustomerContext>
    {
        
        /// <summary>
        /// Contructor used by the business logic
        /// </summary>
        public CustomerDataManager(bool preloading = false, bool lazyLoadingDefault = false, bool tracking = false)
         : this(preloading, lazyLoadingDefault, tracking, null)
        {

        }

        /// <summary>
        /// This constructor must be internal otherwise we need a EF Reference in the busines logic
        /// </summary>
        internal CustomerDataManager(bool preloading = true, bool lazyLoadingDefault = false, bool tracking = false, MobileHubCustomerContext customerContext = null)
        : base(lazyLoadingDefault, tracking, customerContext)
        {
            if (preloading) ctx.Customers.ToList();
        }

        public Customer GetCustomer(string customerId, bool? tracking = null)
        {
            var query = from c in Query<Customer>(tracking) where c.Id == customerId select c;
            return query.SingleOrDefault();
        }


        public List<Customer> GetCustomers(string name)
        {
            var query = from p in ctx.Customers where p.FirstName.Contains(name) || p.LastName.Contains(name) select p;
            return query.ToList();
        }


        public List<Customer> GetCustomerByAccountManager(int accountManagerId)
        {

            return ctx.Customers.Include(c => c.Meetings.Select(m => m.Address).Select(a => a.Country))
                .Include(c => c.CustomerType)
                .Where(c => c.AccountManagersUserId == accountManagerId).ToList();
        }

        public object GetCustomerActionsHistory(int accountManagerId, string localeId)
        {

            var query = from h
                in ctx.ActionHistories
                join a in ctx.AvailableActions on h.ActionCode equals a.ActionCode
                where h.UserId == accountManagerId && a.LocaleId == localeId
                select new
                {
                    h.Address,
                    h.ActionTime,
                    h.CustomerUser,
                    a.Name,
                    a.Description
                };

            return query.ToList();
        }

        public bool AddCustomerToMeeting(string customerId, string meetingId)
        {
            var fm = new MeetingDataManager(context: ctx);

            var meeting = fm.GetMeeting(meetingId, true); // Load Meeting with "TRACKING" on!
            var customer = GetCustomer(customerId, true);

            meeting.Customer = customer;

            int res = ctx.SaveChanges();
            fm.Dispose();
            return true;
        }

        public List<Customer> SaveCustomers(List<Customer> customers, out string statistics)
        {
            return Save(customers, out statistics);
        }
    }
}

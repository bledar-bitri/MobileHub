using System;
using System.Collections.Generic;
using System.Linq;
using CustomerModel;
using DataAccessLayer.HelperClasses;

namespace DataAccessLayer.Managers
{
    /// <summary>
    /// Datamanager for User entities in the customer database
    /// </summary>
    public class CustomerUserDataManager : DataManagerBase<MobileHubCustomerContext>
    {

        /// <summary>
        /// Contructor used by the business logic
        /// </summary>
        public CustomerUserDataManager(bool preloading = false, bool lazyLoadingDefault = false, bool tracking = false)
         : this(preloading, lazyLoadingDefault, tracking, null)
        {

        }

        /// <summary>
        /// This constructor must be internal otherwise we need a EF Reference in the busines logic
        /// </summary>
        internal CustomerUserDataManager(bool preloading = true, bool lazyLoadingDefault = false, bool tracking = false, MobileHubCustomerContext customerContext = null)
        : base(lazyLoadingDefault, tracking, customerContext)
        {
            if (preloading) ctx.CustomerUsers.ToList();
        }


        /// <summary>
        /// Holt einen Passagier
        /// </summary>
        public CustomerUser GetUser(int userId, bool? tracking = null)
        {
            // .OfType<User>() notwendig wegen Vererbung
            var query = from u in Query<CustomerUser>(tracking) where u.Id == userId select u;
            return query.SingleOrDefault();
        }


        /// <summary>
        /// get all users with a certain name
        /// </summary>
        public List<CustomerUser> GetUsers(int userId)
        {
            var query = from u in ctx.CustomerUsers where u.Id == userId select u;
            return query.ToList();
        }

        /// <summary>
        /// get all users 
        /// </summary>
        public List<CustomerUser> GetUsers()
        {
            var query = from user in ctx.CustomerUsers.OfType<CustomerUser>() select user;
            return query.ToList();
        }

        public bool AddUserToMeeting(int userId, string meetingId)
        {

            var mf = new MeetingDataManager(context: ctx); // the abbreviation is NOT a coincidence

            var meeting = mf.GetMeeting(meetingId, true);
            var user = GetUser(userId, true);

            meeting.CustomerUser = user;

            int res = ctx.SaveChanges();
            mf.Dispose();
            return true;

        }

        public List<CustomerUser> SaveUsers(List<CustomerUser> users, out string statistics)
        {
            return Save(users, out statistics);
        }
    }
}

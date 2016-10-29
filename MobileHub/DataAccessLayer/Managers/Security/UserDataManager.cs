using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using DataAccessLayer.HelperClasses;
using SecurityModel;

namespace DataAccessLayer.Managers.Security
{
    /// <summary>
    /// Datamanager for User entities in the customer database
    /// </summary>
    public class UserDataManager : DataManagerBase<MobileHubSecurityContext>
    {

        /// <summary>
        /// Contructor used by the business logic
        /// </summary>
        public UserDataManager(bool preloading = false, bool lazyLoadingDefault = false, bool tracking = false)
         : this(preloading, lazyLoadingDefault, tracking, null)
        {

        }

        /// <summary>
        /// This constructor must be internal otherwise we need a EF Reference in the busines logic
        /// </summary>
        internal UserDataManager(bool preloading = true, bool lazyLoadingDefault = false, bool tracking = false, MobileHubSecurityContext ctx = null)
        : base(lazyLoadingDefault, tracking, ctx)
        {
            if (preloading) ctx.Users.ToList();
        }


        /// <summary>
        /// Holt einen Passagier
        /// </summary>
        public User GetUser(int userId, bool? tracking = null)
        {
            return ctx.Users
                .Include(u => u.Memberships.Select(m => m.Role))
                .Include(u => u.Memberships.Select(m=> m.UserCompany).Select(c=>c.Account))
                .SingleOrDefault(u => u.Id == userId);

        }


        /// <summary>
        /// get all users with a certain name
        /// </summary>
        public List<User> GetUsers(string name)
        {
            var query = from u in ctx.Users where u.FirstName.Contains(name) || u.LastName.Contains(name) select u;
            return query.ToList();
        }

        /// <summary>
        /// get all users 
        /// </summary>
        public List<User> GetUsers()
        {
            var query = from user in ctx.Users select user;
            return query.ToList();
        }
        /*
        public bool AddUserToRole(int userId, int companyId, int roleId)
        {

            var mf = new MeetingDataManager(kontext: ctx); // the abbreviation is NOT a coincidence

            var meeting = mf.GetMeeting(meetingId, true);
            var user = GetUser(userId, true);

            meeting.CustomerUser = user;

            int res = ctx.SaveChanges();
            mf.Dispose();
            return true;

        }*/

        public List<User> SaveUsers(List<User> users, out string statistics)
        {
            return Save(users, out statistics);
        }
    }
}

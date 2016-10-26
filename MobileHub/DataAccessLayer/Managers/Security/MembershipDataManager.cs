using System.Collections.Generic;
using System.Linq;
using DataAccessLayer.HelperClasses;
using LoginDatabaseContext;


namespace DataAccessLayer.Managers.Security
{
    /// <summary>
    /// Datamanager for Membership entities
    /// inherits from DataManagerBase
    /// </summary>
    public class MembershipDataManager : DataManagerBase<MobileHubSecurityContext>
    {

        public MembershipDataManager(bool preloading = false, bool lazyLoading = false, bool tracking = false)
         : this(lazyLoading, tracking, preloading, null)
        {
        }

        /// <summary>
        /// This constructor must be internal otherwise we need a EF Reference in the busines logic
        /// </summary>
        internal MembershipDataManager(bool lazyLoadingDefault = false, bool tracking = false, bool preloading = true, MobileHubSecurityContext ctx = null) :
         base(lazyLoadingDefault, tracking, ctx)
        {
            // Preloading loads all meetings in the context-cache
            if (preloading)
            {
                this.preloading = true;
                ctx.Memberships.ToList();
            }
        }

        public Membership GetMembership(int membershipId, bool? tracking = null, bool includeUser = false, bool includeCustomer = false)
        {

            List<string> includes = null;
            if (includeUser) includes = new List<string>() { "User" };
            var q = Query<Membership, User>(tracking, x => x.User);


            var query = from m in q where m.Id == membershipId select m;
            var meeting = query.SingleOrDefault();
            
            return meeting;
        }

        public List<Membership> GetUserMemberships(int userid, bool noTracking = true)
        {
            var query = from m in Query<Membership>(noTracking, "") select m;

            query = from membership in query
                where membership.UserId == userid
                select membership;
            
            return query.ToList();
        }

        public void Update(Membership membership)
        {
            ctx.Memberships.Add(membership);
            ctx.SaveChanges();
        }

        public int SaveChanges()
        {
            return ctx.SaveChanges();
        }

        public List<Membership> SaveChanges(List<Membership> meetings, out string statistics)
        {
            return Save(meetings, out statistics);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using DataAccessLayer.HelperClasses;
using LoginDatabaseContext;


namespace DataAccessLayer.Managers
{
    /// <summary>
    /// Datenmanager for Meeting entities
    /// inherits from DataManagerBase
    /// </summary>
    public class MeetingDataManager : DataManagerBase<MobileHubCustomerContext>
    {

        public MeetingDataManager(bool preloading = false, bool lazyLoading = false, bool tracking = false)
         : this(lazyLoading, tracking, preloading, null)
        {
        }



        /// <summary>
        /// This constructor must be internal otherwise we need a EF Reference in the busines logic
        /// </summary>
        internal MeetingDataManager(bool lazyLoadingDefault = false, bool tracking = false, bool preloading = true, MobileHubCustomerContext kontext = null) :
         base(lazyLoadingDefault, tracking, kontext)
        {
            // Preloading loads all meetings in the context-cache
            if (preloading)
            {
                this.preloading = true;
                ctx.Meetings.ToList();
            }
        }

        public Meeting GetMeeting(int meetingId, bool? tracking = null, bool includeUsers = false, bool includeCustomers = false)
        {

            List<string> includes = null;
            if (includeUsers) includes = new List<string>() { "CustomerUser" };
            var q = Query<Meeting, CustomerUser>(tracking, x => x.CustomerUser);


            var query = from m in q where m.Id == meetingId select m;
            var meeting = query.SingleOrDefault();
            
            return meeting;
        }
        
        public List<Meeting> GetMeetings(int userId, int customerId, bool noTracking = true)
        {
            var query = from m in Query<Meeting>(noTracking, "") select m;

            query = from m in query
                where m.CustomerId == customerId && m.UserId == userId
                select m;
            
            return query.ToList();
        }
        

        public List<Meeting> GetMeetingsForUser(int userid, bool noTracking = true)
        {
            var query = from m in Query<Meeting>(noTracking, "") select m;

            query = from meeting in query
                where meeting.UserId == userid
                select meeting;
            
            return query.ToList();
        }

        public void Update(Meeting meeting)
        {
            ctx.Meetings.Add(meeting);
            ctx.SaveChanges();
        }

        public int SaveChanges()
        {
            return ctx.SaveChanges();
        }

        /// <summary>
        /// Änderungen an einer Liste von Passagieren speichern
        /// Die neu hinzugefügten Passagiere muss die Routine wieder zurückgeben, da die IDs für die 
        /// neuen Passagiere erst beim SaveChanges von der Datenbank vergeben werden
        /// </summary>
        public List<Meeting> SaveChanges(List<Meeting> meetings, out string statistics)
        {
            return Save<Meeting>(meetings, out statistics);
        }
    }
}

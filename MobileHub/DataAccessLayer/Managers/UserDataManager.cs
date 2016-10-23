using System;
using System.Collections.Generic;
using System.Linq;
using DataAccessLayer.Util;
using DatabaseContext;
using DatabaseContext.Entities;

namespace DataAccessLayer.Managers
{
    /// <summary>
    /// Datenmanager für Passagier-Entitäten
    /// </summary>
    public class UserDataManager : DataManagerBase, IDisposable
    {

        /// <summary>
        /// Öffentlicher Konstruktor für GL
        /// </summary>
        public UserDataManager(bool preloading = false, bool lazyLoadingDefault = false, bool tracking = false)
         : this(preloading, lazyLoadingDefault, tracking, null)
        {

        }

        ///<summary>
        /// Dieser Konstruktur muss internal sein, denn sonst würde die GL eine Referenz auf EF brauchen!!!
        /// </summary>
        internal UserDataManager(bool preloading = true, bool lazyLoadingDefault = false, bool tracking = false, MobileHubContext kontext = null)
        : base(lazyLoadingDefault, tracking, kontext)
        {
            // Preloading lädt alle Flüge in den Cache
            if (preloading) ctx.Users.ToList();
        }


        /// <summary>
        /// Holt einen Passagier
        /// </summary>
        public User GetUser(int userId, bool? tracking = null)
        {
            // .OfType<User>() notwendig wegen Vererbung
            var query = from u in Query<User>(tracking) where u.Id == userId select u;
            return query.SingleOrDefault();
        }


        /// <summary>
        /// get all users with a certain name
        /// </summary>
        public List<User> GetUsers(string name)
        {
            // .OfType<User>() notwendig wegen Vererbung
            var query = from u in ctx.Users.OfType<User>() where u.FirstName.Contains(name) || u.LastName.Contains(name) select u;
            return query.ToList();
        }

        /// <summary>
        /// get all users 
        /// </summary>
        public List<User> GetUsers()
        {
            // .OfType<User>() notwendig wegen Vererbung
            var query = from user in ctx.Users.OfType<User>() select user;
            return query.ToList();
        }

        /// <summary>
        /// Füge einen Passagier zu einem Flug hinzu
        /// </summary>
        public bool AddUserToMeeting(int userId, int meetingId)
        {

            // Flug flug = modell.FlugSet.Where(f => f.Id == meetingId).SingleOrDefault();
            var fm = new MeetingDataManager(kontext: ctx);

            Meeting meeting = fm.GetMeeting(meetingId, true); // Flug unbedingt "TRACKING" laden!
            User user = GetUser(userId, true);

            // Eigentliches Hinzufügen !!!
            //meeting.PassagierSet.Add(user);

            int anz = ctx.SaveChanges();
            fm.Dispose();
            return true;

        }

        /// <summary>
        /// Änderungen an einer Liste von Passagieren speichern
        /// Die neu hinzugefügten Passagiere muss die Routine wieder zurückgeben, da die IDs für die 
        /// neuen Passagiere erst beim SaveChanges von der Datenbank vergeben werden
        /// </summary>
        public List<User> SaveUsers(List<User> users, out string statistics)
        {
            return Save(users, out statistics);
        }




    }
}

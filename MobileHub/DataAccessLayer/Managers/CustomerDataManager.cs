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
    public class CustomerDataManager : DataManagerBase, IDisposable
    {

        /// <summary>
        /// Öffentlicher Konstruktor für GL
        /// </summary>
        public CustomerDataManager(bool preloading = false, bool lazyLoadingDefault = false, bool tracking = false)
         : this(preloading, lazyLoadingDefault, tracking, null)
        {

        }

        ///<summary>
        /// Dieser Konstruktur muss internal sein, denn sonst würde die GL eine Referenz auf EF brauchen!!!
        /// </summary>
        internal CustomerDataManager(bool preloading = true, bool lazyLoadingDefault = false, bool tracking = false, MobileHubContext kontext = null)
        : base(lazyLoadingDefault, tracking, kontext)
        {
            // Preloading lädt alle Flüge in den Cache
            if (preloading) ctx.Users.ToList();
        }


        /// <summary>
        /// Holt einen Passagier
        /// </summary>
        public Customer GetCustomer(int customerId, bool? tracking = null)
        {
            var query = from c in Query<Customer>(tracking) where c.Id == customerId select c;
            return query.SingleOrDefault();
        }


        /// <summary>
        /// Get all Customer by name
        /// </summary>
        public List<Customer> GetCustomers(string name)
        {
            // .OfType<Customer>() notwendig wegen Vererbung
            var abfrage = from p in ctx.Users.OfType<Customer>() where p.FirstName.Contains(name) || p.LastName.Contains(name) select p;
            return abfrage.ToList();
        }

        public bool AddCustomerToMeeting(int customerId, int meetingId)
        {
            var fm = new MeetingDataManager(kontext: ctx);

            Meeting meeting = fm.GetMeeting(meetingId, true); // Load Meeting with "TRACKING" on!
            Customer customer = GetCustomer(customerId, true);

            meeting.Customer = customer;

            int anz = ctx.SaveChanges();
            fm.Dispose();
            return true;
        }

        /// <summary>
        /// Änderungen an einer Liste von Passagieren speichern
        /// Die neu hinzugefügten Passagiere muss die Routine wieder zurückgeben, da die IDs für die 
        /// neuen Passagiere erst beim SaveChanges von der Datenbank vergeben werden
        /// </summary>
        public List<Customer> SaveCustomers(List<Customer> customers, out string statistics)
        {
            return Save(customers, out statistics);
        }




    }
}

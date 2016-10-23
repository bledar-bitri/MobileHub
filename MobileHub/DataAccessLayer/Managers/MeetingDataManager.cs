using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.HelperClasses;
using DatabaseContext;
using DatabaseContext.Entities;

namespace DataAccessLayer.Managers
{
    /// <summary>
    /// Datenmanager für Flug-Entitäten
    /// abgeleitet von DataManagerBase
    /// </summary>
    public class MeetingDataManager : DataManagerBase<MobileHubContext>
    {

        //public MeetingDataManager(bool lazyLoadingDefault = false, bool tracking = false) :
        // base(lazyLoadingDefault, tracking)
        //{
        //}


        public MeetingDataManager(bool preloading = false, bool lazyLoading = false, bool tracking = false)
         : this(lazyLoading, tracking, preloading, null)
        {
        }



        /// <summary>
        /// Dieser Konstruktur muss internal sein, denn sonst würde die GL eine Referenz auf EF brauchen!!!
        /// </summary>
        internal MeetingDataManager(bool lazyLoadingDefault = false, bool tracking = false, bool preloading = true, MobileHubContext kontext = null) :
         base(lazyLoadingDefault, tracking, kontext)
        {
            // Preloading lädt alle Piloten in den Kontext-Cache
            if (preloading)
            {
                this.preloading = true;
                ctx.Meetings.ToList();
            }
        }

        public Meeting GetMeeting(int meetingId, bool? tracking = null, bool includeUsers = false, bool includeCustomers = false)
        {

            List<string> includes = null;
            if (includeUsers) includes = new List<string>() { "User" };
            var q = Query<Meeting, User>(tracking, x => x.User);

            //if (includeCustomers) q = q.Include(p => p.PassagierSet);

            var query = from m in q where m.Id == meetingId select m;
            var meeting = query.SingleOrDefault();
            // CUI.Print("Flug geladen: " + meetingId + " Zustand=" + ctx.Entry(e).State, ConsoleColor.Cyan);


            return meeting;
        }
        
        public List<Meeting> GetMeetings(int userId, int customerId, bool noTracking = true)
        {
            // Nutzung der Query-Hilfsmethode aus der Basisklasse
            var query = from m in Query<Meeting>(noTracking, "") select m;
            // oder
            // var abfrage = from f in Query<Flug, ICollection<Passagier>>(true, f =>f.PassagierSet) select f;

            // Eigentliche Logik für das Zusammensetzen der Abfrage
            query = from m in query
                where m.CustomerID == customerId && m.UserID == userId
                select m;
            
            return query.ToList();
        }
        

        public List<Meeting> GetMeetingsForUser(int userid, bool noTracking = true)
        {
            // Nutzung der Query-Hilfsmethode aus der Basisklasse
            var query = from m in Query<Meeting>(noTracking, "") select m;
            // oder
            // var abfrage = from f in Query<Flug, ICollection<Passagier>>(true, f =>f.PassagierSet) select f;

            // Eigentliche Logik für das Zusammensetzen der Abfrage
            query = from meeting in query
                where meeting.UserID == userid
                select meeting;
            
            return query.ToList();
        }

        public void Update(Meeting meeting)
        {
            ctx.Meetings.Add(meeting);
            ctx.SaveChanges();
        }

/*
        public List<string> GetFlughaefen()
        {
            var l1 = Query<Flug>(false).Select(f => f.Abflugort).Distinct();
            var l2 = Query<Flug>(false).Select(f => f.Zielort).Distinct();
            var l3 = l1.Union(l2).Distinct();
            return l3.OrderBy(z => z).ToList();
        }

    */
        /// <summary>
        /// Implementierung einer Kapselung von SaveChanges() direkt in einer konkreten Datenzugriffsmanagerklasse
        /// Rückgabewert ist die Summe der Anzahl der gespeicherten neuen, geänderten und gelöschten Datensätze
        /// </summary>
        /// <returns></returns>
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

        /*
        public bool ReducePlatzAnzahl(int FlugID, short PlatzAnzahl)
        {
            var einzelnerFlug = GetFlug(FlugID, true);
            if (einzelnerFlug != null)
            {
                if (einzelnerFlug.FreiePlaetze >= PlatzAnzahl)
                {
                    einzelnerFlug.FreiePlaetze -= PlatzAnzahl;
                    ctx.SaveChanges();
                    return true;
                }
            }
            return false;

        }
        */
    }
}

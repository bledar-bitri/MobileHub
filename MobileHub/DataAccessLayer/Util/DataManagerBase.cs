using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using DatabaseContext;

namespace DataAccessLayer.Util
{
    /// <summary>
    /// Basisklasse für alle Datenmanager  / mit EF 6.x
    /// </summary>
    abstract public class DataManagerBase : IDisposable
    {
        // Eine Instanz des Framework-Kontextes pro Manager-Instanz
        protected MobileHubContext ctx;
        protected bool DisposeContext = true;
        // Globale Einstellung für Preloading
        protected bool preloading = false;
        // Globale Einstellung für Lazy Loading
        protected bool lazyLoadingDefault = false;
        // Globale Einstellung für NoTraking
        protected bool trackingDefault = false;

        /// <summary>
        /// Log-Properties nach außen durchreichen
        /// Erwartet ja nur Methode, die string erwartet --> Kein Architekturproblem
        /// </summary>
        public Action<string> Log
        {
            get
            {
                return ctx.Database.Log;
            }
            set
            {
                ctx.Database.Log = value;
            }
        }


        /// <summary>
        /// Der Konstruktor erzeugt die Instanz des EF-Kontextes
        /// </summary>
        //protected DataManagerBase()
        //{
        // this.ctx = new WWWingsModell(); 
        //}

        protected DataManagerBase() : this(false, false, null)
        {
        }

        protected DataManagerBase(bool lazyLoadingDefault = false, bool tracking = false)
        {
            // Setzen der Standards
            this.lazyLoadingDefault = lazyLoadingDefault;
            this.trackingDefault = tracking;

            // (De-)aktiviert das transparent (automatische) Nachladen verbundener Objekte
            this.ctx.Configuration.LazyLoadingEnabled = lazyLoadingDefault;
            //// ... dafür braucht man aber auch Proxies!!!
            //this.ctx.Configuration.ProxyCreationEnabled = lazyLoadingDefault;
        }


        protected DataManagerBase(bool lazyLoadingDefault = false, bool tracking = false, MobileHubContext kontext = null)
        {
            this.lazyLoadingDefault = lazyLoadingDefault;
            this.trackingDefault = tracking;
            // Falls ein Kontext hineingereicht wurde, nehme diesen!
            if (kontext != null) { this.ctx = kontext; DisposeContext = false; }
            else { this.ctx = new MobileHubContext(); }

            // (De-)aktiviert das transparent (automatische) Nachladen verbundener Objekte
            this.ctx.Configuration.LazyLoadingEnabled = lazyLoadingDefault;
            // ... dafür braucht man aber auch Proxies!!!
            this.ctx.Configuration.ProxyCreationEnabled = lazyLoadingDefault;

            //AutoDetectChangesEnabled kann man ggf. abschalten, wenn man Proxies verwendet und alle Properties virtual sind
            //this.ctx.Configuration.AutoDetectChangesEnabled = false;
        }


        /// <summary>
        /// DataManager vernichten (vernichtet auch den EF-Kontext)
        /// </summary>
        public void Dispose()
        {
            // Falls der Kontext von außen hineingereicht wurde, darf man nicht Dispose() aufrufen.
            // Das ist dann Sache des Aufrufers
            if (DisposeContext) ctx.Dispose();
        }

        /// <summary>
        /// Grundabfrage mit NoTracking-Option und optionalen Includes, die als Zeichenkettenliste übergeben werden
        /// </summary>
        protected IQueryable<TEntity> Query<TEntity>(bool? Tracking = null, List<string> includes = null)
         where TEntity : class
        {
            var q = GetBaseQuery<TEntity>(Tracking);
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
        /// Grundabfrage mit NoTracking-Option und einem Include als einzelne Zeichenkette
        /// </summary>
        protected IQueryable<TEntity> Query<TEntity>(bool? Tracking, string include)
         where TEntity : class
        {
            return Query<TEntity>(Tracking, new List<string>() { include });
        }

        /// <summary>
        /// Grundabfrage mit NoTracking-Optionen und optionalen Includes als Expression
        /// </summary>
        protected IQueryable<TEntity> Query<TEntity, TProperty>(bool? Tracking = null, Expression<Func<TEntity, TProperty>> path = null)
         where TEntity : class
        {
            IQueryable<TEntity> q = GetBaseQuery<TEntity>(Tracking);
            if (path != null) // Eager Loading?
            {
                q = q.AsQueryable().Include(path);
            }
            return q;
        }

        /// <summary>
        /// Interne Hilfsmethode, die DbQuery mit oder ohne Tracking liefert
        /// </summary>
        private DbQuery<TEntity> GetBaseQuery<TEntity>(bool? Tracking) where TEntity : class
        {
            DbQuery<TEntity> q = ctx.Set<TEntity>();
            if ((Tracking.HasValue == true && Tracking.Value == false) || (Tracking.HasValue == false && trackingDefault == false)) // Tracking nicht gewünscht
            {
                q = q.AsNoTracking();
            }
            return q;
        }

        /// <summary>
        /// Implementierung von SaveChanges() zur Lebenszeit des Kontextes
        /// Rückgabewert ist eine Zeichenkette, die Informationen über die Anzahl der neuen, geänderten und gelöschten Datensätze enthält
        /// </summary>
        /// <returns></returns>
        public string Save()
        {
            string ergebnis = GetStatistik();
            var anz = ctx.SaveChanges();
            return ergebnis;
        }

        /// <summary>
        /// SaveChanges für losgelöste Entitätsobjekte mit Autowert-Primärschlüssel
        /// Die neu hinzugefügten Objekte muss die SaveChanges-Routine wieder zurückgeben, da die IDs für die 
        /// neuen Objekte erst beim SaveChanges von der Datenbank vergeben werden
        /// </summary>
        protected List<TEntity> Save<TEntity>(IEnumerable<TEntity> menge, out string Statistik)
        where TEntity : class
        {
            var neue = new List<TEntity>();

            // Änderungen für jeden einzelnen Passagier übernehmen
            foreach (dynamic o in menge)
            {
                // Anfügen an diesen Kontext
                ctx.Set<TEntity>().Attach((TEntity)o);
                // Unterscheidung anhand des Primärschlüssels bei Autorwerten
                // 0 -> neu
                if (o.Id == 0)
                {
                    ctx.Entry(o).State = EntityState.Added;
                    // Neue Datensätze merken, da diese nach SaveChanges zurückgegeben werden müssen (haben dann erst ihre IDs!)
                    neue.Add(o);
                }
                else
                {
                    // nicht 0 -> geändert
                    ctx.Entry(o).State = EntityState.Modified;
                }
            }

            // Statistik der Änderungen zusammenstellen
            Statistik = GetStatistik<TEntity>();

            // Änderungen speichern
            var e = ctx.SaveChanges();

            return neue;
        }



        /// <summary>
        /// SaveChanges für losgelöste Entitätsobjekte mit EntityState-Property
        /// Die neu hinzugefügten Objekte muss die SaveChanges-Routine wieder zurückgeben, da die IDs für die 
        /// neuen Objekte erst beim SaveChanges von der Datenbank vergeben werden
        /// </summary>
        protected List<TEntity> SaveEx<TEntity>(IEnumerable<TEntity> menge, out string Statistik)
      where TEntity : class
        {
            var neue = new List<TEntity>();

            // Änderungen für jeden einzelne Objekt aus dessen EntityState übernehmen
            foreach (dynamic o in menge)
            {

                if (o.EntityState == EntityState.Added)
                {

                    ctx.Entry(o).State = EntityState.Added;
                    neue.Add(o);
                }
                if (o.EntityState == EntityState.Deleted)
                {
                    ctx.Set<TEntity>().Attach((TEntity)o);
                    ctx.Set<TEntity>().Remove(o);
                }
                if (o.EntityState == EntityState.Modified)
                {
                    ctx.Set<TEntity>().Attach((TEntity)o);
                    ctx.Entry(o).State = EntityState.Modified;
                }
            }

            // Statistik der Änderungen zusammenstellen
            Statistik = GetStatistik<TEntity>();

            // Änderungen speichern
            var e = ctx.SaveChanges();

            return neue;
        }


        /// <summary>
        /// Liefert Informationen über ChangeTracker-Status als Zeichenkette
        /// </summary>
        protected string GetStatistik<TEntity>()
      where TEntity : class
        {
            string Statistik = "";
            Statistik += "Geändert: " + ctx.ChangeTracker.Entries<TEntity>().Where(x => x.State == EntityState.Modified).Count();
            Statistik += " Neu: " + ctx.ChangeTracker.Entries<TEntity>().Where(x => x.State == EntityState.Added).Count();
            Statistik += " Gelöscht: " + ctx.ChangeTracker.Entries<TEntity>().Where(x => x.State == EntityState.Deleted).Count();
            return Statistik;
        }

        /// <summary>
        /// Liefert Informationen über ChangeTracker-Status als Zeichenkette
        /// </summary>
        protected string GetStatistik()
        {
            string Statistik = "";
            Statistik += "Geändert: " + ctx.ChangeTracker.Entries().Where(x => x.State == EntityState.Modified).Count();
            Statistik += " Neu: " + ctx.ChangeTracker.Entries().Where(x => x.State == EntityState.Added).Count();
            Statistik += " Gelöscht: " + ctx.ChangeTracker.Entries().Where(x => x.State == EntityState.Deleted).Count();
            return Statistik;
        }
    }
}

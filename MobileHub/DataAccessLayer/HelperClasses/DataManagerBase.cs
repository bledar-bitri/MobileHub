using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.HelperClasses
{
    public class DataManagerBase<TDbContext> : IDisposable where TDbContext : DbContext, new()
    {
        // Only one instace of the Framework Context pro Manager 
        protected TDbContext ctx;
        protected bool disposeContext = true;
        // Global setting for Preloading
        protected bool preloading = false;
        // Global setting for Lazy Loading
        protected bool lazyLoadingDefault = false;
        // Global setting for NoTracking
        protected bool trackingDefault = false;

        /// <summary>
        /// Log-Properties reachible from outside
        /// Expects only a method that gets a string as parameter --> No architectural problem
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
        /// The Constructor generates EF-Context instance
        /// </summary>
        //protected DataManagerBase()
        //{
        // this.ctx = new MobileHubContext();
        //}

        protected DataManagerBase()
         : this(false, false, null)
        {
        }

        protected DataManagerBase(bool lazyLoadingDefault = false, bool tracking = false)
        {
            // Set the defaults
            this.lazyLoadingDefault = lazyLoadingDefault;
            this.trackingDefault = tracking;

            // (De-)activate of transparent (automatic) lazyLoading of the tied objects
            this.ctx.Configuration.LazyLoadingEnabled = lazyLoadingDefault;
            //// ... for that we need Proxies as well!!!
            //this.ctx.Configuration.ProxyCreationEnabled = lazyLoadingDefault;
        }


        protected DataManagerBase(bool lazyLoadingDefault = false, bool tracking = false, TDbContext context = null)
        {
            this.lazyLoadingDefault = lazyLoadingDefault;
            this.trackingDefault = tracking;
            // If a context is passed we used it
            if (context != null) { this.ctx = context; disposeContext = false; }
            else { this.ctx = new TDbContext(); }

            // (De-)activate of transparent (automatic) lazyLoading of the tied objects
            this.ctx.Configuration.LazyLoadingEnabled = lazyLoadingDefault;
            // ... for that we need Proxies as well!!!
            this.ctx.Configuration.ProxyCreationEnabled = lazyLoadingDefault;

            //AutoDetectChangesEnabled can be turned off, if one uses Proxies and all properties are declared as virtual
            //this.ctx.Configuration.AutoDetectChangesEnabled = false;
        }


        /// <summary>
        /// Disposes the DataManager as well as the EF-Context
        /// </summary>
        public void Dispose()
        {
            // If the Context is passed to us we should not dispose it
            // The caller should take care of it
            if (disposeContext) ctx.Dispose();
        }

        /// <summary>
        /// Basic query with NoTracking-Option and optional Includes, which are passed as a list of strings
        /// </summary>
        protected IQueryable<TEntity> Query<TEntity>(bool? tracking = null, List<string> includes = null)
         where TEntity : class
        {
            var q = GetBaseQuery<TEntity>(tracking);
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
        /// Basic query with NoTracking-Option and one Include as a single string
        /// </summary>
        protected IQueryable<TEntity> Query<TEntity>(bool? tracking, string include)
         where TEntity : class
        {
            return Query<TEntity>(tracking, new List<string>() { include });
        }

        /// <summary>
        /// Basic query with NoTracking-Option and optional Includes as Expression
        /// </summary>
        protected IQueryable<TEntity> Query<TEntity, TProperty>(bool? tracking = null, Expression<Func<TEntity, TProperty>> path = null)
         where TEntity : class
        {
            IQueryable<TEntity> q = GetBaseQuery<TEntity>(tracking);
            if (path != null) // Eager Loading?
            {
                q = q.AsQueryable().Include(path);
            }
            return q;
        }

        /// <summary>
        /// Internal helper method that provides dbquery with or without tracking
        /// </summary>
        private DbQuery<TEntity> GetBaseQuery<TEntity>(bool? tracking) where TEntity : class
        {
            DbQuery<TEntity> q = ctx.Set<TEntity>();
            if ((tracking.HasValue == true && tracking.Value == false) || (tracking.HasValue == false && trackingDefault == false)) // Tracking nicht gewünscht
            {
                q = q.AsNoTracking();
            }
            return q;
        }



        /// <summary>
        /// Implementation of SaveChanges() during the lifetime of the context
        /// Return value is a string containing information about the number of new, changed, and deleted records
        /// </summary>
        /// <returns></returns>
        public string Save()
        {
            string result = GetStatistics();
            ctx.SaveChanges();
            return result;
        }



        /// <summary>
        /// Save detached entity objects with AutoNumber primary key
        /// The newly added objects must be returned to the called because the IDs are assigned by the database during the Save method
        /// </summary>
        protected List<TEntity> Save<TEntity>(IEnumerable<TEntity> entities, out string statistics)
        where TEntity : class
        {
            var newItemsList = new List<TEntity>();

            // Collect the changes for each entity
            foreach (dynamic o in entities)
            {
                // insert in this context
                ctx.Set<TEntity>().Attach((TEntity)o);
                // distinction based on the primary key for auto-values
                if (o.Id is int)
                {
                    // 0 -> new 
                    if (o.Id == 0)
                    {
                        ctx.Entry(o).State = EntityState.Added;
                        // keep track of new records, because we need to return them after they are saved (because they get their IDs during the save process)
                        newItemsList.Add(o);
                    }
                    else
                    {
                        // not 0 -> changed
                        ctx.Entry(o).State = EntityState.Modified;
                    }
                }

                if (o.Id is string)
                {
                    // null or empty -> new 
                    if (string.IsNullOrEmpty(o.Id))
                    {
                        ctx.Entry(o).State = EntityState.Added;
                        // keep track of new records, because we need to return them after they are saved (because they get their IDs during the save process)
                        newItemsList.Add(o);
                    }
                    else
                    {
                        // -> changed
                        ctx.Entry(o).State = EntityState.Modified;
                    }
                }

            }

            // compile statistics of the changes
            statistics = GetStatistics<TEntity>();

            // Save changes
            var e = ctx.SaveChanges();

            return newItemsList;
        }



        /// <summary>
        /// Save detached entity objects with EntityState Property
        /// The newly added objects must be returned to the called because the IDs are assigned by the database during the Save method
        /// </summary>
        protected List<TEntity> SaveEx<TEntity>(IEnumerable<TEntity> entities, out string statistics)
      where TEntity : class
        {
            var newItemsList = new List<TEntity>();

            // track changes for each object based an its EntityState
            foreach (dynamic o in entities)
            {

                if (o.EntityState == ITVEntityState.Added)
                {

                    ctx.Entry(o).State = EntityState.Added;
                    newItemsList.Add(o);
                }
                if (o.EntityState == ITVEntityState.Deleted)
                {
                    ctx.Set<TEntity>().Attach((TEntity)o);
                    ctx.Set<TEntity>().Remove(o);
                }
                if (o.EntityState == ITVEntityState.Modified)
                {
                    ctx.Set<TEntity>().Attach((TEntity)o);
                    ctx.Entry(o).State = EntityState.Modified;
                }
            }

            // compile statistics of the changes
            statistics = GetStatistics<TEntity>();

            // save changes
            var e = ctx.SaveChanges();

            return newItemsList;
        }


        /// <summary>
        /// Provides information about the ChangeTracker status as a string
        /// </summary>
        protected string GetStatistics<TEntity>()
      where TEntity : class
        {
            string stats = "";
            stats += "Changed: " + ctx.ChangeTracker.Entries<TEntity>().Count(x => x.State == EntityState.Modified);
            stats += " New: " + ctx.ChangeTracker.Entries<TEntity>().Count(x => x.State == EntityState.Added);
            stats += " Deleted: " + ctx.ChangeTracker.Entries<TEntity>().Count(x => x.State == EntityState.Deleted);
            return stats;
        }

        /// <summary>
        /// Provides information about the ChangeTracker status as a string
        /// </summary>
        protected string GetStatistics()
        {
            string stats = "";
            stats += "Changed: " + ctx.ChangeTracker.Entries().Count(x => x.State == EntityState.Modified);
            stats += " New: " + ctx.ChangeTracker.Entries().Count(x => x.State == EntityState.Added);
            stats += " Deleted: " + ctx.ChangeTracker.Entries().Count(x => x.State == EntityState.Deleted);
            return stats;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.HelperClasses
{
    /// <summary>
    /// Basisklasse für alle Datenmanager zur Verwaltung eines bestimmten Entitätstyps  / mit EF 6.x
    /// V1.1 
    /// </summary>
    public abstract class EntityManagerBase<TDbContext, TEntity> : DataManagerBase<TDbContext>
     where TDbContext : DbContext, new()
     where TEntity : class
    {

        public IQueryable<TEntity> Query()
        {
            return this.Query<TEntity>();
        }

        public virtual TEntity GetByID(object id)
        {
            return ctx.Set<TEntity>().Find(id);
        }

        public TEntity Update(TEntity obj)
        {
            ctx.Set<TEntity>().Attach(obj);
            ctx.Entry(obj).State = EntityState.Modified;
            ctx.SaveChanges();
            return obj;
        }

        public TEntity New(TEntity obj)
        {
            ctx.Set<TEntity>().Add(obj);
            ctx.SaveChanges();
            return obj;
        }

        public bool IsLoaded(TEntity obj)
        {
            return ctx.Set<TEntity>().Local.Any(e => e == obj);
        }

        public virtual void Remove(object id)
        {
            TEntity obj = ctx.Set<TEntity>().Find(id);
            Remove(obj);
        }

        public bool Remove(TEntity obj)
        {
            if (!this.IsLoaded(obj)) ctx.Set<TEntity>().Attach(obj);

            ctx.Set<TEntity>().Remove(obj);
            ctx.SaveChanges();
            return true;
        }
    }

}

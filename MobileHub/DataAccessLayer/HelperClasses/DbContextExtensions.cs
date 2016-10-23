using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.HelperClasses
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// Liefert aus einem DbContext ein ObjectSet<T>, über den einige Funktionen zugänglich sind, die es in DbSet<T> leider nicht gibt.
        /// </summary>
        public static ObjectSet<T> GetObjectSet<T>(this DbContext ctx) where T : class
        {
            var objectContext = ((IObjectContextAdapter)ctx).ObjectContext;
            ObjectSet<T> set = objectContext.CreateObjectSet<T>();
            return set;
        }


        /// <summary>
        /// Holt den Namen der Tabelle in DB für einen Entitätstyp
        /// </summary>
        /// <returns></returns>
        public static string GetTableName(this DbContext context, Type type)
        {
            var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

            // Get the part of the model that contains info about the actual CLR types
            var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

            // Get the entity type from the model that maps to the CLR type
            var entityType = metadata
                    .GetItems<EntityType>(DataSpace.OSpace)
                    .Single(e => objectItemCollection.GetClrType(e) == type);

            // Get the entity set that uses this entity type
            var entitySet = metadata
                .GetItems<EntityContainer>(DataSpace.CSpace)
                .Single()
                .EntitySets
                .Single(s => s.ElementType.Name == entityType.Name);

            // Find the mapping between conceptual and storage model for this entity set
            var mapping = metadata.GetItems<EntityContainerMapping>(DataSpace.CSSpace)
                    .Single()
                    .EntitySetMappings
                    .Single(s => s.EntitySet == entitySet);

            // Find the storage entity set (table) that the entity is mapped
            var table = mapping
                .EntityTypeMappings.Single()
                .Fragments.Single()
                .StoreEntitySet;

            // Return the table name from the storage entity set
            return (string)table.MetadataProperties["Table"].Value ?? table.Name;
        }
    }
}

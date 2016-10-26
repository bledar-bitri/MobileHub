using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginDatabaseContext
{
    public class IgnoreEntityStatePropertyConvention : Convention
    {
        private const string Name = "EntityState";

        public IgnoreEntityStatePropertyConvention()
        {
            //var x = Types().Where(t => t.GetProperty(Name) != null && !t.IsSubclassOf(typeof(Person)));

            //x.Configure(c => c.Ignore(Name));
        }
    }
}

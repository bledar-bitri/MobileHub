
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using MobileHub.Models;

namespace MobileHub.DAL
{
    public class MobileHubContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        //public MobileHubContext() : base("MobileHubContext")
        public MobileHubContext() : base()
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            Database.SetInitializer<MobileHubContext>(new MobileHubInitializer());
        }
    }
}

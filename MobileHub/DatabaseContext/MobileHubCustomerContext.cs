
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using DatabaseContext.Entities;

namespace DatabaseContext
{
    public class MobileHubCustomerContext : DbContext
    {
        //public MobileHubCustomerContext() : base("MobileHubCustomerContext")
        public MobileHubCustomerContext() : base()
        {
            
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();
            return base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Conventions.Add<IgnoreEntityStatePropertyConvention>();
        }


        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Meeting> Meetings { get; set; }

    }
}

using System.Data.Entity.ModelConfiguration.Conventions;

namespace CustomerModel
{
    using System.Data.Entity;

    public partial class MobileHubCustomerContext : DbContext
    {
        public MobileHubCustomerContext()
            : base("name=MobileHubCustomerContext")
        {
        }

        public virtual DbSet<ActionHistory> ActionHistories { get; set; }
        public virtual DbSet<ActionType> ActionTypes { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<AvailableAction> AvailableActions { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerCompany> CustomerCompanies { get; set; }
        public virtual DbSet<CustomerParentCompany> CustomerParentCompanies { get; set; }
        public virtual DbSet<CustomerType> CustomerTypes { get; set; }
        public virtual DbSet<CustomerUser> CustomerUsers { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Locale> Locales { get; set; }
        public virtual DbSet<Meeting> Meetings { get; set; }
        public virtual DbSet<OrderHeader> OrderHeaders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            
            modelBuilder.Entity<ActionHistory>()
                .Property(e => e.Memo)
                .IsUnicode(false);

            modelBuilder.Entity<ActionType>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<ActionType>()
                .HasMany(e => e.AvailableActions)
                .WithRequired(e => e.ActionType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Address>()
                .Property(e => e.Street)
                .IsUnicode(false);

            modelBuilder.Entity<Address>()
                .Property(e => e.Street2)
                .IsUnicode(false);

            modelBuilder.Entity<Address>()
                .Property(e => e.City)
                .IsUnicode(false);

            modelBuilder.Entity<Address>()
                .Property(e => e.Sate)
                .IsUnicode(false);

            modelBuilder.Entity<Address>()
                .Property(e => e.Zip)
                .IsUnicode(false);

            modelBuilder.Entity<Address>()
                .HasMany(e => e.ActionHistories)
                .WithRequired(e => e.Address)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Address>()
                .HasMany(e => e.Customers)
                .WithRequired(e => e.Address)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Address>()
                .HasMany(e => e.CustomerCompanies)
                .WithRequired(e => e.Address)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Address>()
                .HasMany(e => e.Events)
                .WithRequired(e => e.Address)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Address>()
                .HasMany(e => e.Meetings)
                .WithRequired(e => e.Address)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Address>()
                .HasMany(e => e.OrderHeaders)
                .WithRequired(e => e.Address)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AvailableAction>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<AvailableAction>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Country>()
                .Property(e => e.Abbreviation)
                .IsUnicode(false);

            modelBuilder.Entity<Country>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Country>()
                .HasMany(e => e.Addresses)
                .WithRequired(e => e.Country)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Customer>()
                .Property(e => e.FirstName)
                .IsUnicode(false);

            modelBuilder.Entity<Customer>()
                .Property(e => e.MiddleName)
                .IsUnicode(false);

            modelBuilder.Entity<Customer>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            modelBuilder.Entity<Customer>()
                .Property(e => e.OriginalCustomerId)
                .IsUnicode(false);

            modelBuilder.Entity<Customer>()
                .HasMany(e => e.ActionHistories)
                .WithRequired(e => e.Customer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Customer>()
                .HasMany(e => e.Meetings)
                .WithRequired(e => e.Customer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Customer>()
                .HasMany(e => e.OrderHeaders)
                .WithRequired(e => e.Customer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CustomerCompany>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<CustomerCompany>()
                .HasMany(e => e.Customers)
                .WithRequired(e => e.CustomerCompany)
                .HasForeignKey(e => e.CompanyId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CustomerParentCompany>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<CustomerParentCompany>()
                .HasMany(e => e.CustomerCompanies)
                .WithOptional(e => e.CustomerParentCompany)
                .HasForeignKey(e => e.ParentCompanyId);

            modelBuilder.Entity<CustomerType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<CustomerType>()
                .HasMany(e => e.AvailableActions)
                .WithRequired(e => e.CustomerType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CustomerType>()
                .HasMany(e => e.Customers)
                .WithRequired(e => e.CustomerType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CustomerUser>()
                .HasMany(e => e.ActionHistory)
                .WithRequired(e => e.CustomerUser)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CustomerUser>()
                .HasMany(e => e.Customers)
                .WithRequired(e => e.CustomerUser)
                .HasForeignKey(e => e.AccountManagersUserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CustomerUser>()
                .HasMany(e => e.Events)
                .WithRequired(e => e.CustomerUser)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CustomerUser>()
                .HasMany(e => e.Meetings)
                .WithRequired(e => e.CustomerUser)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CustomerUser>()
                .HasMany(e => e.OrderHeaders)
                .WithRequired(e => e.CustomerUser)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Event>()
                .Property(e => e.Purpose)
                .IsUnicode(false);

            modelBuilder.Entity<Event>()
                .Property(e => e.Memo)
                .IsUnicode(false);

            modelBuilder.Entity<Item>()
                .Property(e => e.OriginalItemId)
                .IsUnicode(false);

            modelBuilder.Entity<Item>()
                .Property(e => e.Title)
                .IsUnicode(false);

            modelBuilder.Entity<Item>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Item>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Item>()
                .Property(e => e.PicUrl)
                .IsUnicode(false);

            modelBuilder.Entity<Item>()
                .HasMany(e => e.OrderItems)
                .WithRequired(e => e.Item)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Locale>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<Locale>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Locale>()
                .Property(e => e.LcidString)
                .IsUnicode(false);

            modelBuilder.Entity<Locale>()
                .HasMany(e => e.AvailableActions)
                .WithRequired(e => e.Locale)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Locale>()
                .HasMany(e => e.CustomerUsers)
                .WithRequired(e => e.Locale)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Meeting>()
                .Property(e => e.Purpose)
                .IsUnicode(false);

            modelBuilder.Entity<Meeting>()
                .Property(e => e.Memo)
                .IsUnicode(false);

            modelBuilder.Entity<OrderHeader>()
                .Property(e => e.TotalAmount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<OrderHeader>()
                .Property(e => e.TotalTax)
                .HasPrecision(19, 4);

            modelBuilder.Entity<OrderHeader>()
                .HasMany(e => e.OrderItems)
                .WithRequired(e => e.OrderHeader)
                .HasForeignKey(e => e.OrderId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OrderItem>()
                .Property(e => e.ItemPrice)
                .HasPrecision(19, 4);

            modelBuilder.Entity<OrderItem>()
                .Property(e => e.TotalPrice)
                .HasPrecision(19, 4);
        }
    }
}

namespace SecurityModel
{
    using System.Data.Entity;

    public partial class MobileHubSecurityContext : DbContext
    {
        public MobileHubSecurityContext()
            : base("name=MobileHubSecurityContext")
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Login> Logins { get; set; }
        public virtual DbSet<Membership> Memberships { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserCompany> UserCompanies { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.PlanLevel)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .HasMany(e => e.UserCompanies)
                .WithRequired(e => e.Account)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Login>()
                .Property(e => e.UserName)
                .IsUnicode(false);

            modelBuilder.Entity<Login>()
                .Property(e => e.PasswordHash)
                .IsUnicode(false);

            modelBuilder.Entity<Membership>()
                .Property(e => e.AccountEmailAddress)
                .IsUnicode(false);

            modelBuilder.Entity<Membership>()
                .Property(e => e.AccountPhoneNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Membership>()
                .Property(e => e.AccountFax)
                .IsUnicode(false);

            modelBuilder.Entity<Role>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Role>()
                .HasMany(e => e.Memberships)
                .WithRequired(e => e.Role)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .Property(e => e.FirstName)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.UserName)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.MiddleName)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.EMailAddress)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Memberships)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserCompany>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<UserCompany>()
                .Property(e => e.AccessLevel)
                .IsUnicode(false);

            modelBuilder.Entity<UserCompany>()
                .HasMany(e => e.Memberships)
                .WithRequired(e => e.UserCompany)
                .WillCascadeOnDelete(false);
        }
    }
}

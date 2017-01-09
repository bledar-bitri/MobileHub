namespace PopulateMobileHub.htb
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class HTBContext : DbContext
    {
        public HTBContext()
            : base("name=HTBContext")
        {
        }

        public virtual DbSet<tblGegnerAdressen> tblGegnerAdressens { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tblGegnerAdressen>()
                .Property(e => e.GALatitude)
                .HasPrecision(18, 5);

            modelBuilder.Entity<tblGegnerAdressen>()
                .Property(e => e.GALongitude)
                .HasPrecision(18, 5);

            modelBuilder.Entity<tblGegnerAdressen>()
                .Property(e => e.GADescription)
                .IsUnicode(false);
        }
    }
}

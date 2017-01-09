namespace RouteModel
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MobileHubRouteContext : DbContext
    {
        public MobileHubRouteContext()
            : base("name=MobileHubRouteContext")
        {
        }

        public virtual DbSet<RoadInfo> RoadInfos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}

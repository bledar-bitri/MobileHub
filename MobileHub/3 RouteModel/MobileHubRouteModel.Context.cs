﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RouteModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class MobileHubRouteContext : DbContext
    {
        public MobileHubRouteContext()
            : base("name=MobileHubRouteContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<RoadInfo> RoadInfos { get; set; }
    
        public virtual int AddRoadInfo(string fromAddr, Nullable<int> fromLat, Nullable<int> fromLon, string toAddr, Nullable<int> toLat, Nullable<int> toLon, Nullable<int> distance, Nullable<int> timeInSeconds)
        {
            var fromAddrParameter = fromAddr != null ?
                new ObjectParameter("fromAddr", fromAddr) :
                new ObjectParameter("fromAddr", typeof(string));
    
            var fromLatParameter = fromLat.HasValue ?
                new ObjectParameter("fromLat", fromLat) :
                new ObjectParameter("fromLat", typeof(int));
    
            var fromLonParameter = fromLon.HasValue ?
                new ObjectParameter("fromLon", fromLon) :
                new ObjectParameter("fromLon", typeof(int));
    
            var toAddrParameter = toAddr != null ?
                new ObjectParameter("toAddr", toAddr) :
                new ObjectParameter("toAddr", typeof(string));
    
            var toLatParameter = toLat.HasValue ?
                new ObjectParameter("toLat", toLat) :
                new ObjectParameter("toLat", typeof(int));
    
            var toLonParameter = toLon.HasValue ?
                new ObjectParameter("toLon", toLon) :
                new ObjectParameter("toLon", typeof(int));
    
            var distanceParameter = distance.HasValue ?
                new ObjectParameter("distance", distance) :
                new ObjectParameter("distance", typeof(int));
    
            var timeInSecondsParameter = timeInSeconds.HasValue ?
                new ObjectParameter("timeInSeconds", timeInSeconds) :
                new ObjectParameter("timeInSeconds", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("AddRoadInfo", fromAddrParameter, fromLatParameter, fromLonParameter, toAddrParameter, toLatParameter, toLonParameter, distanceParameter, timeInSecondsParameter);
        }
    }
}

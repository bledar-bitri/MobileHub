//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class RoadInfo
    {
        public long FromLatitude { get; set; }
        public long FromLongitude { get; set; }
        public long ToLatitude { get; set; }
        public long ToLongitude { get; set; }
        public double Distance { get; set; }
        public long TimeInSeconds { get; set; }
        public Nullable<System.DateTime> LookupDate { get; set; }
    }
}
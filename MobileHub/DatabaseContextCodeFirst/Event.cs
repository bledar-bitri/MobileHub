namespace DatabaseContextCodeFirst
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Event")]
    public partial class Event
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime EventTime { get; set; }

        [StringLength(255)]
        public string Purpose { get; set; }

        [Column(TypeName = "text")]
        public string Memo { get; set; }

        public int AddressId { get; set; }

        public int? CustomerId { get; set; }

        public virtual Address Address { get; set; }

        public virtual CustomerUser CustomerUser { get; set; }
    }
}

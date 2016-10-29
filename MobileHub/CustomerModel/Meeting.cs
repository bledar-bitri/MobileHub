namespace CustomerModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.Azure.Mobile.Server;

    public partial class Meeting : EntityData
    {

        public int UserId { get; set; }

        public string CustomerId { get; set; }

        public DateTime MeetingTime { get; set; }

        [StringLength(255)]
        public string Purpose { get; set; }

        [Column(TypeName = "text")]
        public string Memo { get; set; }

        public string AddressId { get; set; }

        public virtual Address Address { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual CustomerUser CustomerUser { get; set; }
    }
}

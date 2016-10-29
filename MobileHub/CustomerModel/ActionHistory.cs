namespace CustomerModel
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.Azure.Mobile.Server;

    public partial class ActionHistory : EntityData
    {
        public string CustomerId { get; set; }

        public int UserId { get; set; }

        public string AddressId { get; set; }

        public int ActionCode { get; set; }

        public DateTime ActionTime { get; set; }

        [Column(TypeName = "text")]
        public string Memo { get; set; }

        public virtual Address Address { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual CustomerUser CustomerUser { get; set; }
    }
}

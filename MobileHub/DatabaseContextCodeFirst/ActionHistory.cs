namespace DatabaseContextCodeFirst
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ActionHistory")]
    public partial class ActionHistory
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int UserId { get; set; }

        public int AddressId { get; set; }

        public int ActionCode { get; set; }

        public DateTime ActionTime { get; set; }

        [Column(TypeName = "text")]
        public string Memo { get; set; }

        public virtual Address Address { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual CustomerUser CustomerUser { get; set; }
    }
}

namespace DatabaseContextCodeFirst
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OrderHeader")]
    public partial class OrderHeader
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrderHeader()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int AddressId { get; set; }

        public int UserId { get; set; }

        public int? MeetingId { get; set; }

        public int? EventId { get; set; }

        public DateTime OrderTime { get; set; }

        [Column(TypeName = "money")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "money")]
        public decimal TotalTax { get; set; }

        public virtual Address Address { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual CustomerUser CustomerUser { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}

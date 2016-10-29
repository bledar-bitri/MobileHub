//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LoginDatabaseContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrderHeader
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrderHeader()
        {
            this.OrderItems = new HashSet<OrderItem>();
        }
    
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int AddressId { get; set; }
        public int UserId { get; set; }
        public Nullable<int> MeetingId { get; set; }
        public Nullable<int> EventId { get; set; }
        public System.DateTime OrderTime { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalTax { get; set; }
    
        public virtual Address Address { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual CustomerUser CustomerUser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
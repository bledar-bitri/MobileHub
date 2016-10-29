namespace CustomerModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.Azure.Mobile.Server;
    
    public partial class Customer : EntityData
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Customer()
        {
            ActionHistories = new HashSet<ActionHistory>();
            Meetings = new HashSet<Meeting>();
            OrderHeaders = new HashSet<OrderHeader>();
        }

        
        public string CompanyId { get; set; }

        [Required]
        [StringLength(200)]
        public string FirstName { get; set; }

        [StringLength(200)]
        public string MiddleName { get; set; }

        [Required]
        [StringLength(200)]
        public string LastName { get; set; }

        public string AddressId { get; set; }

        public int AccountManagersUserId { get; set; }

        [StringLength(100)]
        public string OriginalCustomerId { get; set; }

        public string CustomerTypeId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActionHistory> ActionHistories { get; set; }

        public virtual Address Address { get; set; }

        public virtual CustomerCompany CustomerCompany { get; set; }

        public virtual CustomerType CustomerType { get; set; }

        public virtual CustomerUser CustomerUser { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Meeting> Meetings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderHeader> OrderHeaders { get; set; }
    }
}

namespace CustomerModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.Azure.Mobile.Server;
    
    public partial class CustomerCompany : EntityData
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CustomerCompany()
        {
            Customers = new HashSet<Customer>();
        }
        
        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public string AddressId { get; set; }

        public string ParentCompanyId { get; set; }

        public virtual Address Address { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Customer> Customers { get; set; }

        public virtual CustomerParentCompany CustomerParentCompany { get; set; }
    }
}

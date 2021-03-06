namespace DatabaseContextCodeFirst
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Address")]
    public partial class Address
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Address()
        {
            ActionHistories = new HashSet<ActionHistory>();
            Customers = new HashSet<Customer>();
            CustomerCompanies = new HashSet<CustomerCompany>();
            Events = new HashSet<Event>();
            Meetings = new HashSet<Meeting>();
            OrderHeaders = new HashSet<OrderHeader>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Street { get; set; }

        [StringLength(255)]
        public string Street2 { get; set; }

        [Required]
        [StringLength(255)]
        public string City { get; set; }

        [StringLength(255)]
        public string Sate { get; set; }

        [Required]
        [StringLength(10)]
        public string Zip { get; set; }

        public int CountryId { get; set; }

        public long? Latitude { get; set; }

        public long? Longitude { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActionHistory> ActionHistories { get; set; }

        public virtual Country Country { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Customer> Customers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CustomerCompany> CustomerCompanies { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Event> Events { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Meeting> Meetings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderHeader> OrderHeaders { get; set; }
    }
}

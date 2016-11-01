
namespace CustomerModel
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.Azure.Mobile.Server;
    public partial class Address : EntityData
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]

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

        public string CountryId { get; set; }

        public long? Latitude { get; set; }

        public long? Longitude { get; set; }

        public virtual Country Country { get; set; }
    }
}

namespace CustomerModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.Azure.Mobile.Server;
    
    public partial class Country : EntityData
    {
        
        [Required]
        [StringLength(10)]
        public string Abbreviation { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }
    }
}

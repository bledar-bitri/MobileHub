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
    
    public partial class Locale
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Locale()
        {
            this.AvailableActions = new HashSet<AvailableAction>();
            this.CustomerUsers = new HashSet<CustomerUser>();
        }
    
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string LcidString { get; set; }
        public Nullable<int> LcidDecimal { get; set; }
        public Nullable<int> LcidHex { get; set; }
        public Nullable<int> LcidCodePage { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AvailableAction> AvailableActions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CustomerUser> CustomerUsers { get; set; }
    }
}

namespace SecurityModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Membership")]
    public partial class Membership
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int UserCompanyId { get; set; }

        public int RoleId { get; set; }

        [Required]
        [StringLength(150)]
        public string AccountEmailAddress { get; set; }

        [StringLength(20)]
        public string AccountPhoneNumber { get; set; }

        [StringLength(20)]
        public string AccountFax { get; set; }

        public virtual Role Role { get; set; }

        public virtual User User { get; set; }

        public virtual UserCompany UserCompany { get; set; }
    }
}

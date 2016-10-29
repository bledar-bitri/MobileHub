namespace DatabaseContextCodeFirst
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AvailableAction")]
    public partial class AvailableAction
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public int LocaleId { get; set; }

        public int CustomerTypeId { get; set; }

        public int ActionTypeId { get; set; }

        public int ActionCode { get; set; }

        public virtual ActionType ActionType { get; set; }

        public virtual CustomerType CustomerType { get; set; }

        public virtual Locale Locale { get; set; }
    }
}

namespace CustomerModel
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.Azure.Mobile.Server;

    public partial class AvailableAction : EntityData
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public string LocaleId { get; set; }

        public string CustomerTypeId { get; set; }

        public string ActionTypeId { get; set; }

        public int ActionCode { get; set; }

        public virtual ActionType ActionType { get; set; }

        public virtual CustomerType CustomerType { get; set; }

        public virtual Locale Locale { get; set; }
    }
}

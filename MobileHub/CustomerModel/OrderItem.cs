namespace CustomerModel
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.Azure.Mobile.Server;
    
    public partial class OrderItem : EntityData
    {

        public string OrderId { get; set; }

        public string ItemId { get; set; }

        public string ItemQuantity { get; set; }

        [Column(TypeName = "money")]
        public decimal ItemPrice { get; set; }

        [Column(TypeName = "money")]
        public decimal TotalPrice { get; set; }

        public virtual Item Item { get; set; }

        public virtual OrderHeader OrderHeader { get; set; }
    }
}

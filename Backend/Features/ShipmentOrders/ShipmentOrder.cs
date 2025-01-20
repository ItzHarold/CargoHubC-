using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Backend.Features.Orders;
using Backend.Features.Shipments;

namespace Backend.Features.ShipmentOrders
{
    [Table("ShipmentOrders")]
    public class ShipmentOrder : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Shipment")]
        [JsonPropertyName("shipment_id")]
        public int ShipmentId { get; set; }

        [Required]
        [ForeignKey("Order")]
        [JsonPropertyName("order_id")]
        public int OrderId { get; set; }

        // Navigation properties
        public virtual Order Order { get; set; } = null!;
        public virtual Shipment Shipment { get; set; } = null!;
    }
}

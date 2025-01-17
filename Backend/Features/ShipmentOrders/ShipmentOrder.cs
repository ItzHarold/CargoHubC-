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
        public required int Id { get; set; }

        [ForeignKey("Shipments")]
        [JsonPropertyName("shipmentId")]
        public required int shipmentId { get; set; }

        [ForeignKey("Orders")]
        [JsonPropertyName("order_id")]
        public required int orderId { get; set; }

        public Order? order { get; set; } = null!;
        public Shipment? shipment { get; set; } = null!;

    }
}

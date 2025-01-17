using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Backend.Features.Clients;
using Backend.Features.Contacts;
using Backend.Features.Items;
using Backend.Features.OrderItems;
using Backend.Features.ShipmentOrders;
using Backend.Features.Suppliers;
using Backend.Features.Warehouses;

namespace Backend.Features.Orders
{
    [Table("Orders")]
    public class Order : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [ForeignKey("SourceContact")]
        [JsonPropertyName("source_id")]
        public required int SourceId { get; set; }

        // Navigation property for the supplier contact
        public Contact? SourceContact { get; set; }

        [Required]
        [JsonPropertyName("order_date")]
        public required DateTime OrderDate { get; set; }

        [JsonPropertyName("request_date")]
        public DateTime? RequestDate { get; set; }

        [JsonPropertyName("reference")]
        public string? Reference { get; set; }

        [JsonPropertyName("reference_extra")]
        public string? ReferenceExtra { get; set; }

        [Required]
        [JsonPropertyName("order_status")]
        public required string OrderStatus { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }

        [JsonPropertyName("shipping_notes")]
        public string? ShippingNotes { get; set; }

        [JsonPropertyName("picking_notes")]
        public string? PickingNotes { get; set; }

        [Required]
        [ForeignKey("Warehouse")]
        [JsonPropertyName("warehouse_id")]
        public required int WarehouseId { get; set; }

        // [ForeignKey("Client")]
        // [JsonPropertyName("ship_to")]
        // public string? ShipTo { get; set; }

        // [Required]
        // [ForeignKey("Client")]
        // [JsonPropertyName("bill_to")]
        // public required string BillTo { get; set; }

        public int? ShipmentId { get; set; }

        [JsonPropertyName("total_amount")]
        public float TotalAmount { get; set; }

        [JsonPropertyName("total_discount")]
        public float? TotalDiscount { get; set; }

        [JsonPropertyName("total_tax")]
        public float? TotalTax { get; set; }

        [JsonPropertyName("total_surcharge")]
        public float? TotalSurcharge { get; set; }

        [JsonPropertyName("order_items")]
        public ICollection<OrderItem>? OrderItems { get; set; }  = null!;

        [JsonPropertyName("shipment_orders")]
        public ICollection<ShipmentOrder>? shipmentOrders { get; } = [];

        public Supplier Supplier { get; set; } = null!;

        public Warehouse Warehouse { get; set; } = null!;

        //TODO to be changed
        [JsonPropertyName("items")]
        public List<Item>? Items { get; set; }

        public int? ShipToClientId { get; set; }
        public int? BillToClientId { get; set; }
        public Client? ShipTo {get; set;}
        public Client? BillTo {get; set;}
    }
}

using Newtonsoft.Json;

namespace Backend.Request;

public class OrderRequest
{
    [JsonProperty("source_id")]
    public required int SourceId { get; set; }

    [JsonProperty("order_date")]
    public required DateTime OrderDate { get; set; }

    [JsonProperty("request_date")]
    public DateTime? RequestDate { get; set; }

    [JsonProperty("reference")]
    public string? Reference { get; set; }

    [JsonProperty("reference_extra")]
    public string? ReferenceExtra { get; set; }

    [JsonProperty("order_status")]
    public required string OrderStatus { get; set; }

    [JsonProperty("notes")]
    public string? Notes { get; set; }

    [JsonProperty("ship_to")]
    public int? ShipTo { get; set; }

    [JsonProperty("bill_to")]
    public int? BillTo { get; set; }

    [JsonProperty("shipment_id")]
    public int? ShipmentId { get; set; }

    [JsonProperty("shipping_notes")]
    public string? ShippingNotes { get; set; }

    [JsonProperty("picking_notes")]
    public string? PickingNotes { get; set; }

    [JsonProperty("warehouse_id")]
    public required int WarehouseId { get; set; }

    [JsonProperty("total_amount")]
    public float TotalAmount { get; set; }

    [JsonProperty("total_discount")]
    public float? TotalDiscount { get; set; }

    [JsonProperty("total_tax")]
    public float? TotalTax { get; set; }

    [JsonProperty("total_surcharge")]
    public float? TotalSurcharge { get; set; }

    [JsonProperty("items")]
    public List<OrderItemRequest>? Items { get; set; }
}

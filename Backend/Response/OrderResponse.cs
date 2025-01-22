using Newtonsoft.Json;

namespace Backend.Response;

public class OrderResponse
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("source_id")]
    public int SourceId { get; set; }

    [JsonProperty("order_date")]
    public DateTime OrderDate { get; set; }

    [JsonProperty("request_date")]
    public DateTime? RequestDate { get; set; }

    [JsonProperty("reference")]
    public string? Reference { get; set; }

    [JsonProperty("reference_extra")]
    public string? ReferenceExtra { get; set; }

    [JsonProperty("order_status")]
    public string? OrderStatus { get; set; }

    [JsonProperty("notes")]
    public string? Notes { get; set; }

    [JsonProperty("shipping_notes")]
    public string? ShippingNotes { get; set; }

    [JsonProperty("picking_notes")]
    public string? PickingNotes { get; set; }

    [JsonProperty("warehouse_id")]
    public int WarehouseId { get; set; }

    [JsonProperty("total_amount")]
    public float TotalAmount { get; set; }

    [JsonProperty("total_discount")]
    public float? TotalDiscount { get; set; }

    [JsonProperty("total_tax")]
    public float? TotalTax { get; set; }

    [JsonProperty("total_surcharge")]
    public float? TotalSurcharge { get; set; }

    [JsonProperty("items")]
    public List<OrderItemResponse>? Items { get; set; }
}


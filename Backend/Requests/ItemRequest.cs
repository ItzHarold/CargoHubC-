using Backend.Features.Suppliers;
using Newtonsoft.Json;

namespace Backend.Request;

public class ItemRequest
{
    [JsonProperty("uid")]
    public required string Uid { get; set; }

    [JsonProperty("code")]
    public required string Code { get; set; }

    [JsonProperty("description")]
    public required string Description { get; set; }

    [JsonProperty("short_description")]
    public string? ShortDescription { get; set; }

    [JsonProperty("upc_code")]
    public string? UpcCode { get; set; }

    [JsonProperty("model_number")]
    public string? ModelNumber { get; set; }

    [JsonProperty("commodity_code")]
    public string? CommodityCode { get; set; }

    [JsonProperty("item_line")]
    public int? ItemLineId { get; set; }

    [JsonProperty("item_group")]
    public int? ItemGroupId { get; set; }

    [JsonProperty("item_type")]
    public int? ItemTypeId { get; set; }

    [JsonProperty("unit_purchase_quantity")]
    public int UnitPurchaseQuantity { get; set; }

    [JsonProperty("unit_order_quantity")]
    public int UnitOrderQuantity { get; set; }

    [JsonProperty("pack_order_quantity")]
    public int PackOrderQuantity { get; set; }

    [JsonProperty("supplier_id")]
    public required int SupplierId { get; set; }

    [JsonProperty("supplier_code")]
    public string? SupplierCode { get; set; }

    [JsonProperty("supplier_part_number")]
    public required string SupplierPartNumber { get; set; }
}

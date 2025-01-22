using Newtonsoft.Json;

namespace Backend.Response;

public class InventoryResponse
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("item_id")]
    public required string ItemId { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("item_reference")]
    public string? ItemReference { get; set; }

    [JsonProperty("locations")]
    public required int[] LocationId { get; set; }

    [JsonProperty("total_on_hand")]
    public required int TotalOnHand { get; set; }

    [JsonProperty("total_expected")]
    public required int TotalExpected { get; set; }

    [JsonProperty("total_ordered")]
    public required int TotalOrdered { get; set; }

    [JsonProperty("total_allocated")]
    public required int TotalAllocated { get; set; }

    [JsonProperty("total_available")]
    public required int TotalAvailable { get; set; }
}

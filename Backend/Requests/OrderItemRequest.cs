using Newtonsoft.Json;

namespace Backend.Request;

public class OrderItemRequest
{
    [JsonProperty("item_id")]
    public required string ItemUid { get; set; }

    [JsonProperty("amount")]
    public required int Amount { get; set; }
}

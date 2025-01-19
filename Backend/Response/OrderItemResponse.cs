using Newtonsoft.Json;

namespace Backend.Response;

public class OrderItemResponse
{
    [JsonProperty("item_id")]
    public string? ItemUid { get; set; }

    [JsonProperty("amount")]
    public int Amount { get; set; }
}

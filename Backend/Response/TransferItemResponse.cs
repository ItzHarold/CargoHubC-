using Newtonsoft.Json;

namespace Backend.Response;

public class TransferItemResponse
{
    [JsonProperty("item_id")]
    public required string ItemId { get; set; }

    [JsonProperty("amount")]
    public required int Amount { get; set; }
}

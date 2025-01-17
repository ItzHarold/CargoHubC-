using Newtonsoft.Json;

namespace Backend.Response;

public class TransferRequest
{
    [JsonProperty("reference")]
    public string? Reference { get; set; }

    [JsonProperty("transfer_from")]
    public int? TransferFrom { get; set; }

    [JsonProperty("transfer_to")]
    public int? TransferTo { get; set; }

    [JsonProperty("transfer_status")]
    public string? TransferStatus { get; set; }

    [JsonProperty("items")]
    public required ICollection<TransferItemRequest> Items { get; set; }
}

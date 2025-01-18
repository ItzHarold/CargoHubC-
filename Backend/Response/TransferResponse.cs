using Newtonsoft.Json;
using System;

namespace Backend.Response
{
    public class TransferResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("reference")]
        public string? Reference { get; set; }

        [JsonProperty("transfer_from")]
        public int? TransferFrom { get; set; }

        [JsonProperty("transfer_to")]
        public int? TransferTo { get; set; }

        [JsonProperty("transfer_status")]
        public string? TransferStatus { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("items")]
        public ICollection<TransferItemResponse>? Items { get; set; }
    }

    public class TransferItemResponse
    {
        [JsonProperty("item_id")]
        public string ItemUid { get; set; } = null!;

        [JsonProperty("amount")]
        public int Amount { get; set; }
    }
}

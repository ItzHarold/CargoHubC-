using Newtonsoft.Json;

namespace Backend.Requests
{
    public class ShipmentRequest
    {
        [JsonProperty("order_id")]
        public required int OrderId { get; set; }

        [JsonProperty("source_id")]
        public required int SourceId { get; set; }

        [JsonProperty("order_date")]
        public DateTime? OrderDate { get; set; }

        [JsonProperty("request_date")]
        public DateTime? RequestDate { get; set; }

        [JsonProperty("shipment_date")]
        public DateTime? ShipmentDate { get; set; }

        [JsonProperty("shipment_type")]
        public required string ShipmentType { get; set; }

        [JsonProperty("shipment_status")]
        public string? ShipmentStatus { get; set; }

        [JsonProperty("notes")]
        public string? Notes { get; set; }

        [JsonProperty("carrier_code")]
        public required string CarrierCode { get; set; }

        [JsonProperty("carrier_description")]
        public string? CarrierDescription { get; set; }

        [JsonProperty("service_code")]
        public required string ServiceCode { get; set; }

        [JsonProperty("payment_type")]
        public required string PaymentType { get; set; }

        [JsonProperty("transfer_mode")]
        public required string TransferMode { get; set; }

        [JsonProperty("total_package_count")]
        public int? TotalPackageCount { get; set; }

        [JsonProperty("total_package_weight")]
        public float? TotalPackageWeight { get; set; }

        [JsonProperty("items")]
        public ICollection<ShipmentItemRequest>? ShipmentItems { get; set; } = new List<ShipmentItemRequest>();
    }

    public class ShipmentItemRequest
    {
        [JsonProperty("item_id")]
        public required string ItemId { get; set; }

        [JsonProperty("amount")]
        public required int Amount { get; set; }
    }
}

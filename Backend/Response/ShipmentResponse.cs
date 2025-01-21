using Newtonsoft.Json;

namespace Backend.Requests
{
    public class ShipmentResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
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

        [JsonProperty("shipment_items")]
        public ICollection<ShipmentItemResponse>? ShipmentItems { get; set; }

        [JsonProperty("order_ids")]
        public ICollection<int> OrderIds { get; set; } = new List<int>();
    }

    public class ShipmentItemResponse
    {
        [JsonProperty("item_id")]
        public string? ItemId { get; set; }  // Corrected field name

        [JsonProperty("amount")]
        public int Amount { get; set; }
    }
}

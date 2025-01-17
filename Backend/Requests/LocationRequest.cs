using Newtonsoft.Json;

namespace Backend.Request;

public class LocationRequest
{
    [JsonProperty("warehouse_id")]
    public required int WarehouseId { get; set; }

    [JsonProperty("code")]
    public required string Code { get; set; }

    [JsonProperty("name")]
    public required string Name { get; set; }
}

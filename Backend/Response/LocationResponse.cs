using Newtonsoft.Json;

namespace Backend.Response;

public class LocationResponse
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("warehouse_id")]
    public required int WarehouseId { get; set; }

    [JsonProperty("code")]
    public required string Code { get; set; }

    [JsonProperty("name")]
    public required string Name { get; set; }
}

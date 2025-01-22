using Newtonsoft.Json;

namespace Backend.Requests;

public class WarehouseRequest
{
    [JsonProperty("code")]
    public required string Code { get; set; }

    [JsonProperty("name")]
    public required string Name { get; set; }

    [JsonProperty("address")]
    public required string Address { get; set; }

    [JsonProperty("zip")]
    public required string Zip { get; set; }

    [JsonProperty("city")]
    public required string City { get; set; }

    [JsonProperty("province")]
    public required string Province { get; set; }

    [JsonProperty("country")]
    public required string Country { get; set; }

    [JsonProperty("contacts")]
    public required List<ContactRequest> Contacts { get; set; }
    public object? Locations { get; internal set; }
}

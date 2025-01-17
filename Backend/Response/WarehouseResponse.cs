using Newtonsoft.Json;
using Backend.Requests;

namespace Backend.Response;

public class WarehouseResponse
{
    [JsonProperty("id")]
    public required int Id { get; set; }

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
}

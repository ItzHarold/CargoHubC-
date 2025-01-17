using Newtonsoft.Json;

namespace Backend.Response;

public class ClientRequest
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public required string Name { get; init; }

    [JsonProperty("address")]
    public required string Address { get; init; }

    [JsonProperty("city")]
    public required string City { get; set; }

    [JsonProperty("zip_code")]
    public required string ZipCode { get; set; }

    [JsonProperty("province")]
    public required string Province { get; set; }

    [JsonProperty("country")]
    public required string Country { get; set; }

    [JsonProperty("contact_name")]
    public required string ContactName { get; set; }

    [JsonProperty("contact_phone")]
    public required string ContactPhone { get; set; }

    [JsonProperty("contact_email")]
    public required string ContactEmail { get; set; }
}


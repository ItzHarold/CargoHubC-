using Newtonsoft.Json;

namespace Backend.Response;

public class SupplierRequest
{
    [JsonProperty("code")]
    public required string Code { get; set; }

    [JsonProperty("name")]
    public required string Name { get; set; }

    [JsonProperty("address")]
    public required string Address { get; set; }

    [JsonProperty("address_extra")]
    public string? AddressExtra { get; set; }

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

    [JsonProperty("phone_number")]
    public required string PhoneNumber { get; set; }

    [JsonProperty("reference")]
    public string? Reference { get; set; }
}

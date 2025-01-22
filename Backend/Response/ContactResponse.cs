using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;


namespace Backend.Response;

public class ContactResponse
{
    [JsonProperty("contact_name")]
    public required string ContactName { get; set; }

    [JsonProperty("contact_phone")]
    public required string ContactPhone { get; set; }

    [EmailAddress]
    [JsonProperty("contact_email")]
    public required string ContactEmail { get; set; }
}

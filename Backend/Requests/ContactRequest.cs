using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;


namespace Backend.Requests;

public class ContactRequest
{
    [JsonProperty("contact_name")]
    public required string ContactName { get; set; }

    [JsonProperty("contact_phone")]
    public required string ContactPhone { get; set; }

    [EmailAddress]
    [JsonProperty("contact_email")]
    public required string ContactEmail { get; set; }
}

using Newtonsoft.Json;

namespace Backend.Response;

public class ItemGroupResponse
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public required string Name { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }
}

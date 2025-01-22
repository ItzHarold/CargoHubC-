using Newtonsoft.Json;

namespace Backend.Requests
{
    public class ItemGroupRequest
    {
        [JsonProperty("name")]
        public required string Name { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }
    }
}

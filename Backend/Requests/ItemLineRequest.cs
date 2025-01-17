using Newtonsoft.Json;

namespace Backend.Requests
{
    public class ItemLineRequest
    {
        [JsonProperty("name")]
        public required string Name { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }
    }
}

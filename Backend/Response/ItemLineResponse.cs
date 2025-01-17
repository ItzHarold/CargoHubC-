using Newtonsoft.Json;

namespace Backend.Requests
{
    public class ItemLineResponse
    {
        internal int id;

        [JsonProperty("name")]
        public required string Name { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend.Features.Clients
{
    [Table("Clients")]
    public class Client : BaseEntity
    {
        [JsonPropertyName("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [JsonPropertyName("name")]
        public required string Name { get; init; }

        [Required]
        [JsonPropertyName("address")]
        public required string Address { get; init; }

        [Required]
        [JsonPropertyName("city")]
        public required string City { get; set; }

        [Required]
        [JsonPropertyName("zip_code")]
        public required string ZipCode { get; set; }

        [Required]
        [JsonPropertyName("province")]
        public required string Province { get; set; }

        [Required]
        [JsonPropertyName("country")]
        public required string Country { get; set; }

        [ForeignKey("Contacts")]
        [JsonPropertyName("contact_id")]
        public required string ContactId { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Backend.Features.Orders;

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

        [Required]
        [JsonPropertyName("contact_name")]
        public required string ContactName { get; set; }

        [Required]
        [JsonPropertyName("contact_phone")]
        public required string ContactPhone { get; set; }

        [Required]
        [JsonPropertyName("contact_email")]
        public required string ContactEmail { get; set; }

        public ICollection<Order>? BillTo{get;set;}
        public ICollection<Order>? ShipTo{get;set;}
    }
}

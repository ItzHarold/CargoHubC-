using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Backend.Features.Items;

namespace Backend.Features.Suppliers
{
    [Table("Suppliers")]
    public class Supplier : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [JsonPropertyName("code")]
        public required string Code { get; set; }

        [Required]
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [Required]
        [JsonPropertyName("address")]
        public required string Address { get; set; }

        [JsonPropertyName("address_extra")]
        public string? AddressExtra { get; set; }

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
        [JsonPropertyName("phone_number")]
        public required string PhoneNumber { get; set; }

        [JsonPropertyName("reference")]
        public string? Reference { get; set; }

        public ICollection<Item>? Items{get; set;} = new List<Item>();
    }
}

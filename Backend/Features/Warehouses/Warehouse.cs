using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Backend.Features.Contacts;
using Backend.Features.WarehouseContacts;

namespace Backend.Features.Warehouses
{
    [Table("Warehouses")]
    public class Warehouse : BaseEntity
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

        [Required]
        [JsonPropertyName("zip")]
        public required string Zip { get; set; }

        [Required]
        [JsonPropertyName("city")]
        public required string City { get; set; }

        [Required]
        [JsonPropertyName("province")]
        public required string Province { get; set; }

        [Required]
        [JsonPropertyName("country")]
        public required string Country { get; set; }

        [Required]
        [JsonPropertyName("contact")]
        public required List<Contact> Contacts { get; set; }
    }
}

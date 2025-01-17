using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Backend.Features.Contacts;
using Backend.Features.Locations;
using Backend.Features.Orders;
using Backend.Features.Transfers;
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
        public string? Code { get; set; }

        [Required]
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [Required]
        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [Required]
        [JsonPropertyName("zip")]
        public string? Zip { get; set; }

        [Required]
        [JsonPropertyName("city")]
        public string? City { get; set; }

        [Required]
        [JsonPropertyName("province")]
        public string? Province { get; set; }

        [Required]
        [JsonPropertyName("country")]
        public string? Country { get; set; }

        // Navigation properties
        public ICollection<Transfer> TransfersTo { get; set; } = new List<Transfer>();
        public ICollection<Transfer> TransfersFrom { get; set; } = new List<Transfer>();

        public ICollection<Location>? Locations { get; set; }
        public ICollection<WarehouseContact> WarehouseContacts { get; set; } = new List<WarehouseContact>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}

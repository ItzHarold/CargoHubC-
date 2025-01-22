using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend.Features.Docks
{
    [Table("Docks")]
    public class Dock
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("status")]
        public string Status { get; set; } = "unoccupied";

        [JsonPropertyName("shipment_id")]
        public int ShipmentId { get; set; } = 0; // Default to 0 (unoccupied)

        [Required]
        [JsonPropertyName("warehouse_id")]
        public int WarehouseId { get; set; } // Immutable after creation

        [Required]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}

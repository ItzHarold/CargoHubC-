using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Backend.Features.Inventories;
using Backend.Features.Locations;

namespace Backend.Features.InventoryLocations
{
    [Table("InventoryLocations")]
    public class InventoryLocation : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public required int Id { get; set; }

        [JsonPropertyName("inventory_id")]
        public required int InventoryId { get; set; }

        [JsonPropertyName("location_id")]
        public required int LocationId { get; set; }

        public Inventory Inventory { get; set; } = null!;
        public Location Location { get; set; } = null!;
    }
}

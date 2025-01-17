using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
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

        [ForeignKey("Inventories")]
        [JsonPropertyName("inventory_id")]
        public required int InventoryId { get; set; }

        [ForeignKey("Locations")]
        [JsonPropertyName("location_id")]
        public required int LocationId { get; set; }
    }
}

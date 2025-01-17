using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Backend.Features.InventoryLocations;
using Backend.Features.Transfers;
using Backend.Features.Warehouses;

namespace Backend.Features.Locations
{
    [Table("Locations")]
    public class Location : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [JsonPropertyName("warehouse_id")]
        public required int WarehouseId { get; set; }
        public Warehouse? Warehouse {get; set;} // Navigation property

        [Required]
        [JsonPropertyName("code")]
        public required string Code { get; set; }

        [Required]
        [JsonPropertyName("row")]
        public required string Row { get; set; }

        [Required]
        [JsonPropertyName("rack")]
        public required string Rack { get; set; }

        [Required]
        [JsonPropertyName("shelf")]
        public required string Shelf { get; set; }

        public ICollection<InventoryLocation> InventoryLocations { get; } = [];

        public ICollection<Transfer>? TransfersTo{get;set;}
        public ICollection<Transfer>? TransfersFrom{get;set;}
    }
}

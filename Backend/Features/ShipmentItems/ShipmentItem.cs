using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Backend.Features.Items;
using Backend.Features.Shipments;

namespace Backend.Features.ShimpentItems
{
    [Table("ShipmentItems")]
    public class ShipmentItem : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("ItemUid")]
        public required string ItemUid { get; set; }

        [JsonPropertyName("shipment_id")]
        public required int ShipmentId { get; set; }

        [Required]
        [JsonPropertyName("amount")]
        public required int Amount { get; set; }
        //TODO Implement the feature to decrease an amount from the total

        //Navigator
        public Shipment Shipment { get; set; } = null!;
        public Item Item { get; set; } = null!;

    }
}

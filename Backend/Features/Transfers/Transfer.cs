using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Backend.Features.Items;
using Backend.Features.TransferItems;
using Backend.Features.Warehouses;

namespace Backend.Features.Transfers
{
    [Table("Transfers")]
    public class Transfer : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("reference")]
        public string? Reference { get; set; }

        [ForeignKey("WarehouseFrom")]
        [JsonPropertyName("transfer_from")]
        public int? TransferFrom { get; set; }
        public Warehouse? WarehouseFrom { get; set; }

        [ForeignKey("WarehouseTo")]
        [JsonPropertyName("transfer_to")]
        public int? TransferTo { get; set; }
        public Warehouse? WarehouseTo { get; set; }

        [JsonPropertyName("transfer_status")]
        public string? TransferStatus { get; set; }

        [Required]
        [JsonPropertyName("transfer_items")]
        public ICollection<TransferItem> TransferItems { get; set; } = new List<TransferItem>();
    }
}

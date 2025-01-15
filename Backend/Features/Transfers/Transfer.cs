using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Backend.Features.Items;

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

        //TODO if transfer from is null, transfer to is required. Else if transfer to is null, transfer from is required
        [ForeignKey("WarehouseFrom")]
        [JsonPropertyName("transfer_from")]
        public int? TransferFrom { get; set; }

        [ForeignKey("WarehouseTo")]
        [JsonPropertyName("transfer_to")]
        public int? TransferTo { get; set; }

        [JsonPropertyName("transfer_status")]
        public string? TransferStatus { get; set; }

        [Required]
        [JsonPropertyName("items")]
        public  required List<Item> Items { get; set; }
    }
}

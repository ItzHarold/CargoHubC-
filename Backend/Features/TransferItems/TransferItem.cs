using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Backend.Features.Transfers;

namespace Backend.Features.TransferItem
{
    [Table("TransferItems")]
    public class TransferItems : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public required int Id { get; set; }

        [ForeignKey("Transfers")]
        [JsonPropertyName("transfer_id")]
        public required int TransferId { get; set; }

        [ForeignKey("Items")]
        [JsonPropertyName("ItemUid")]
        public required string ItemUid { get; set; }

        [Required]
        [JsonPropertyName("amount")]
        public required int Amount { get; set; }
    }
}

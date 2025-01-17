using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Backend.Features.Items;
using Backend.Features.Locations;
using Backend.Features.TransferItems;

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
        public int? TransferFromLocationId { get; set; }
        public int? TransferToLocationId { get; set; }
        public Location? TransferTo {get; set;}
        public Location? TransferFrom {get; set;}

        [JsonPropertyName("transfer_status")]
        public string? TransferStatus { get; set; }

        [Required]
        [JsonPropertyName("transfer_items")]
        public ICollection<TransferItem>? TransferItems { get; set; } = new List<TransferItem>();
    }
}

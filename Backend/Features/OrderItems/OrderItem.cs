using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Backend.Features.Orders;

namespace Backend.Features.OrderItem
{
    [Table("OrderItems")]
    public class OrderItem : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public required int Id { get; set; }

        [ForeignKey("Items")]
        [JsonPropertyName("ItemUid")]
        public required string ItemUid { get; set; }

        [ForeignKey("Orders")]
        [JsonPropertyName("order_id")]
        public required int OrderId { get; set; }

        [Required]
        [JsonPropertyName("amount")]
        public required int Amount { get; set; }
        //TODO Implement the feature to decrease an amount from the total
    }
}

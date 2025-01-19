using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Backend.Features.Items;
using Backend.Features.Orders;

namespace Backend.Features.OrderItems
{
    [Table("OrderItems")]
    public class OrderItem : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [JsonPropertyName("ItemUid")]
        public required string ItemUid { get; set; }

        [Required]
        [JsonPropertyName("order_id")]
        public required int OrderId { get; set; }

        [Required]
        [JsonPropertyName("amount")]
        public required int Amount { get; set; }
        //TODO Implement the feature to decrease an amount from the total

        //Navigation Property
        public Order Order { get; set; } = null!;
        public Item Item { get; set; } = null!;
    }
}

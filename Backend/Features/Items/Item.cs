using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Backend.Features.TransferItem;

namespace Backend.Features.Items
{
    [Table("Items")]
    public class Item : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("uid")]
        public required string Uid { get; set; }

        [Required]
        [JsonPropertyName("code")]
        public required string Code { get; set; }

        [Required]
        [JsonPropertyName("description")]
        public required string Description { get; set; }

        [JsonPropertyName("short_description")]
        public string? ShortDescription { get; set; }

        [JsonPropertyName("upc_code")]
        public string? UpcCode { get; set; }

        [JsonPropertyName("model_number")]
        public string? ModelNumber { get; set; }

        [JsonPropertyName("commodity_code")]
        public string? CommodityCode { get; set; }

        [ForeignKey("ItemLine")]
        [JsonPropertyName("item_line")]
        public int? ItemLine { get; set; }

        [ForeignKey("ItemGroup")]
        [JsonPropertyName("item_group")]
        public  int? ItemGroup { get; set; }

        [ForeignKey("ItemType")]
        [JsonPropertyName("item_type")]
        public int? ItemType { get; set; }

        [Required]
        [JsonPropertyName("unit_purchase_quantity")]
        public int UnitPurchaseQuantity { get; set; }

        [Required]
        [JsonPropertyName("unit_order_quantity")]
        public int UnitOrderQuantity { get; set; }

        [Required]
        [JsonPropertyName("pack_order_quantity")]
        public int PackOrderQuantity { get; set; }

        [Required]
        [ForeignKey("Supplier")]
        [JsonPropertyName("supplier_id")]
        public required int SupplierId { get; set; }

        [JsonPropertyName("supplier_code")]
        public string? SupplierCode { get; set; }

        [Required]
        [JsonPropertyName("supplier_part_number")]
        public required string SupplierPartNumber { get; set; }

        [JsonPropertyName("transfer_items")]
        public virtual ICollection<TransferItems> TransferItems { get; set; } = new List<TransferItems>();
    }
}

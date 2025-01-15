using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Backend.Features.Warehouses;

namespace Backend.Features.WarehouseContacts
{
    [Table("WarehouseContacts")]
    public class WarehouseContacts : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [ForeignKey("Warehouses")]
        [JsonPropertyName("warehouse_id")]
        public required int WarehouseId { get; set; }

        [ForeignKey("Contacts")]
        [JsonPropertyName("contact_id")]
        public required int ContactId { get; set; }
        // name - phone number - email
    }
}

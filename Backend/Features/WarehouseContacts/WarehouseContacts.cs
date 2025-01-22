using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Backend.Features.Contacts;
using Backend.Features.Warehouses;

namespace Backend.Features.WarehouseContacts
{
    [Table("WarehouseContacts")]
    public class WarehouseContact : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }
        // name - phone number - email
        [JsonPropertyName("warehouse_id")]
        public int WarehouseId { get; set; }

        [JsonPropertyName("contact_id")]
        public int ContactId { get; set; }

        // NAVIGATOR
        public Warehouse Warehouse { get; set; } = null!;
        public Contact Contact { get; set; } = null!;
    }
}

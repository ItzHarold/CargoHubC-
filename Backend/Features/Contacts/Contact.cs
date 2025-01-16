using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Backend.Features.WarehouseContacts;

namespace Backend.Features.Contacts
{
    [Table("Contacts")]
    public class Contact
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [JsonPropertyName("contact_name")]
        public required string ContactName { get; set; }

        [Required]
        [JsonPropertyName("contact_phone")]
        public required string ContactPhone { get; set; }

        [Required]
        [EmailAddress]
        [JsonPropertyName("contact_email")]
        public required string ContactEmail { get; set; }

        public ICollection<WarehouseContact>? WarehouseContacts { get; set; }
    }
}

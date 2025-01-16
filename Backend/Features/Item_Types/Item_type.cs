using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Features.Items;

namespace Backend.Features.ItemTypes
{
    [Table("Item_Types")]
    public class ItemType : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public string? Description { get; set; }

        public ICollection<Item>? Items {get; set;}
    }
}

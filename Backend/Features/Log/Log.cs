using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Features.Logs
{
    [Table("Log")]
    public class Log
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string? ApiKey { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public string? RequestType { get; set; }

        [Required]
        public string? ResponeType { get; set; }

        public string? RequestBody { get; set; }
    }
}

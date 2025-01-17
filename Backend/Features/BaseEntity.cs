using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Backend.Features;

public abstract class BaseEntity
{
    [Required]
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

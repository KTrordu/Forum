using System.ComponentModel.DataAnnotations;

namespace Forum.Entities;

public abstract class BaseEntity
{
    [Key]
    public int Id { get; set; }

    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public DateTime? DeletedAt { get; set; }
}

namespace Forum.Entities;

public abstract class BaseEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public bool IsDeleted { get; set; } = false;
}

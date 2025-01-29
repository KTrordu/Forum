namespace Forum.Entities;

public abstract class BaseEntity
{
    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }
}

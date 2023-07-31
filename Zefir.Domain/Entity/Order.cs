namespace Zefir.Domain.Entity;

public class Order
{
    public int Id { get; set; }
    public User User { get; set; }
    public ICollection<Product> Products { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; }
    public DateOnly Deadline { get; set; }

    public Order(DateOnly deadline)
    {
        Status = (int)Entity.Status.Default;
        Deadline = deadline;
        CreatedAt = DateTime.Now;
    }
}

public enum Status
{
    Failed = -1,
    Default = 0,
    InWork = 1,
    Done = 2
}

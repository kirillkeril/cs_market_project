namespace Zefir.Core.Entity;

public class Order
{
    public int Id { get; set; }
    public User? User { get; set; }
    public ICollection<Product> Products { get; set; } = null!;
    public int Status { get; set; }
    public DateTime CreatedAt { get; }
    public DateOnly Deadline { get; set; }
    public double Sum { get; set; }

    public Order(DateOnly deadline)
    {
        Status = (int)Entity.Status.Default;
        Deadline = deadline;
        CreatedAt = DateTime.Now;
    }
}

public enum Status
{
    Rejected = -1,
    Default = 0,
    InWork = 1,
    Done = 2
}

public static class StatusExtensions
{
    public static string ToString(this Status status)
    {
        switch (status)
        {
            case Status.Rejected: return nameof(Status.Rejected);
            case Status.InWork: return nameof(Status.InWork);
            case Status.Done: return nameof(Status.Done);
            default: return nameof(Status.Default);
        }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zefir.Domain.Entity;

public class Order
{
    [Key] public int Id { get; }
    [ForeignKey(nameof(User))] public int UserId { get; set; }
    [ForeignKey(nameof(Product))] public int[] ProductsId { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; }
    public DateTime Deadline { get; set; }

    public Order(int userId, int[] productsId, Status status, DateTime deadline)
    {
        UserId = userId;
        ProductsId = productsId;
        Status = (int)status;
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

namespace Zefir.Core.Entity;

public class Basket
{
    public int Id { get; set; }
    public User User { get; init; }
    public List<Product> Products { get; set; } = null!;
}

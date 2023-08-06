namespace Zefir.Core.Entity;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<Characteristics>? Characteristics { get; set; } = new();
    public DateTime CreatedAt { get; }
    public string? ImageFilePath { get; set; }
    public Category Category { get; set; } = null!;
    public double Price { get; set; }

    public Product(string name, string description, double price, string? imageFilePath)
    {
        Name = name;
        Description = description;
        Price = price;
        ImageFilePath = imageFilePath;
        CreatedAt = DateTime.Now;
    }
}

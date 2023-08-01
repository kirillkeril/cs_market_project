namespace Zefir.Core.Entity;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<Characteristics> Characteristics { get; set; } = new();
    public DateTime CreatedAt { get; }
    public string? ImageFilePath { get; set; }
    public Category Category { get; set; } = null!;

    public Product(string name, string description, string? imageFilePath)
    {
        Name = name;
        Description = description;
        ImageFilePath = imageFilePath;
        CreatedAt = DateTime.Now;
    }
}

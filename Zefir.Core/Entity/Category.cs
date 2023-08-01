namespace Zefir.Core.Entity;

public class Category
{
    private string _name;
    private string _description;

    public Category(string name, string description)
    {
        _name = name;
        _description = description;
        CreatedAt = DateTime.Now;
    }

    public Guid Id { get; set; }

    public string Name
    {
        get => _name;
        set
        {
            if (!string.IsNullOrWhiteSpace(value)) _name = value;
            else throw new ArgumentException("Name can't be empty");
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            if (!string.IsNullOrWhiteSpace(value)) _description = value;
            else throw new ArgumentException("Description can't be empty");
        }
    }

    public ICollection<Product>? Products { get; set; }

    public DateTime CreatedAt { get; }
}

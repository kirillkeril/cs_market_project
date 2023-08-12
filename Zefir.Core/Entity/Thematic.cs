using System.ComponentModel.DataAnnotations;

namespace Zefir.Core.Entity;

public class Thematic
{
    [Key] public string Name { get; set; }
    public List<Product> Products { get; set; }
}

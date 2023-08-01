using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zefir.Core.Entity;

public class Role
{
    [NotMapped] public const string UserRole = "user";
    [NotMapped] public const string AdminRole = "admin";
    [Key] public string Name { get; set; }

    public Role(string name)
    {
        Name = name;
    }
}

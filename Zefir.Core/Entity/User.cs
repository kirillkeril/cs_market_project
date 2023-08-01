using System.Text.RegularExpressions;

namespace Zefir.Core.Entity;

public class User
{
    public int Id { get; set; }
    private string _name;
    private string _surname;
    private string _phone;
    private string _email;
    private string _hashedPassword;
    private double? _sale;

    public User(string name, string surname, string phone, string email, string hashedPassword, double? sale = 0)
    {
        _name = name;
        _surname = surname;
        _phone = phone;
        _email = email;
        _hashedPassword = hashedPassword;
        _sale = sale;
    }

    public string Name
    {
        get => _name;
        set
        {
            if (value == string.Empty) throw new ArgumentException("this field can't be null or empty string");
            _name = value;
        }
    }

    public string Surname
    {
        get => _surname;
        set
        {
            if (value == string.Empty) throw new ArgumentException("this field can't be null ir empty string");
            _surname = value;
        }
    }

    public string Phone
    {
        get => _phone;
        set
        {
            var regEx = new Regex(@"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$");
            if (regEx.IsMatch(value)) _phone = value;
            else throw new ArgumentException("this field can't be null ir empty string");
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            if (!string.IsNullOrWhiteSpace(value)) _email = value;
            else throw new ArgumentException("this field can't be null");
        }
    }

    public string HashedPassword
    {
        get => _hashedPassword;
        set
        {
            if (!string.IsNullOrWhiteSpace(value)) _hashedPassword = value;
            else throw new ArgumentException("this field can't be null");
        }
    }

    public string? RefreshToken { get; set; }

    public Role Role { get; set; } = null!;
}

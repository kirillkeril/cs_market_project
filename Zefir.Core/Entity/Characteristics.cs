namespace Zefir.Core.Entity;

public class Characteristics
{
    public int CharacteristicsId { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }

    public Characteristics(string key, string value)
    {
        Key = key;
        Value = value;
    }
}

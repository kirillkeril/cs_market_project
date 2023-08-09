namespace Zefir.Common.Errors;

public class ServiceBadRequestError : Exception
{
    public Dictionary<string, string> FieldErrors { get; } = new();

    public ServiceBadRequestError(params (string, string)[] errors)
    {
        foreach (var error in errors)
        {
            error.ToTuple().Deconstruct(out var field, out var fieldError);
            FieldErrors.Add(field, fieldError);
        }
    }
}

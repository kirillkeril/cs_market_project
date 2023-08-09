namespace Zefir.Common.Errors;

public class ServiceNotFoundError : Exception
{
    public new string Message { get; }

    public ServiceNotFoundError(string message)
    {
        Message = message;
    }
}

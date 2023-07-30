namespace Zefir.DAL.Errors;

public class ServiceNotFoundError : Exception
{
    public string Message { get; }

    public ServiceNotFoundError(string message)
    {
        Message = message;
    }
}

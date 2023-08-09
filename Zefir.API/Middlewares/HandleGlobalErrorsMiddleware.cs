using Zefir.Common.Errors;

namespace Zefir.API.Middlewares;

/// <summary>
/// </summary>
public class HandleGlobalErrorsMiddleware : IMiddleware
{
    private readonly ILogger<HandleGlobalErrorsMiddleware> _logger;

    /// <summary>
    /// </summary>
    /// <param name="logger"></param>
    public HandleGlobalErrorsMiddleware(ILogger<HandleGlobalErrorsMiddleware> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ServiceNotFoundError e)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsJsonAsync(new
            {
                errors = new List<string> { e.Message }
            });
        }
        catch (ServiceBadRequestError e)
        {
            Console.WriteLine(e);
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new
            {
                errors = e.FieldErrors
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new
            {
                errors = new List<string> { $"Server error: {e.Message};" }
            });
        }
    }
}

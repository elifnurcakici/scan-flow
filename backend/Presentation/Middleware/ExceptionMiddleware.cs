using backend.Exceptions;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            await HandleAppException(context, ex);
        }
        catch (Exception ex)
        {
            await HandleUnknownException(context, ex);
        }
    }

    private static Task HandleAppException(HttpContext context, AppException ex)
    {
        context.Response.StatusCode = ex.StatusCode;
        var response = ApiResponse<object>.FailResponse(
            ex.Message,
            null,
            context.TraceIdentifier
        );

        return context.Response.WriteAsJsonAsync(response);
    }

    private static Task HandleUnknownException(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = 500;
        var response = ApiResponse<object>.FailResponse(
            "Something went wrong.",
            null,
            context.TraceIdentifier
        );

        return context.Response.WriteAsJsonAsync(response);
    }
}

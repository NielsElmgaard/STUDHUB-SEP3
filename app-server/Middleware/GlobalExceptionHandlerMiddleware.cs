using System.Text.Json;
using StudHub.SharedDTO.StoreCredentials;

public class GlobalExceptionHandlerMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(
        ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (InvalidOperationException ex)
        {
            await HandleGenericExceptionAsync(context, ex, 404);
        }
        catch (ArgumentException ex)
        {
            await HandleStoreExceptionAsync(context, ex, 400);
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleGenericExceptionAsync(context, ex, 401);
        }
        catch (Exception ex)
        {
            await HandleGenericExceptionAsync(context, ex, 500);
        }
    }

    
    private async Task HandleStoreExceptionAsync(HttpContext context,
        ArgumentException exception, int statusCode)
    {
        var traceId = context.TraceIdentifier;

        _logger.LogError(exception,
            "BrickLink or Brick Owl credential validation failed. TraceId: {TraceId}",
            traceId);

        var errorResponse = new BrickLinkCredentialsResponseDTO
        {
            IsSucces = false,
            ErrorMessage = exception.Message
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var jsonResponse = JsonSerializer.Serialize(errorResponse);
        await context.Response.WriteAsync(jsonResponse);
    }

    private async Task HandleGenericExceptionAsync(HttpContext context,
        Exception exception, int statusCode)
    {
        var traceId = context.TraceIdentifier;
        _logger.LogError(exception, "Error occurred. TraceId: {TraceId}",
            traceId);

        var genericError = new
        {
            Title = "Server Error",
            Status = statusCode,
            Detail = exception.Message,
            TraceId = traceId
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(
            JsonSerializer.Serialize(genericError));
    }
}
using System.Net.Sockets;
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
        catch (ArgumentException ex)
        {
            await HandleStoreExceptionAsync(context, ex, 400);
        }
        catch (Exception ex)
        {
            await HandleAllExceptionsAsync(context, ex);
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

    private async Task HandleAllExceptionsAsync(HttpContext context,
        Exception exception)
    {
        var traceId = context.TraceIdentifier;

        _logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}",
            traceId);

        var errorResponse = new BrickLinkCredentialsResponseDTO
        {
            IsSucces = false,
            ErrorMessage = exception.Message
        };

        context.Response.ContentType = "application/json";

        int statusCode = exception switch
        {
            HttpRequestException => 503,
            SocketException => 503,
            InvalidOperationException => 503,
            ArgumentException => 400,
            _ => 500
        };

        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(errorResponse));
    }
}
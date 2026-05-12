using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace backend.Middleware;
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        var traceId = context.TraceIdentifier;
        var method = context.Request.Method;
        var path = context.Request.Path;

        _logger.LogInformation("Incoming Request {Method} {Path} TraceId: {TraceId}",
        method, path, traceId);

        await _next(context);

        stopwatch.Stop();

        var statusCode = context.Response.StatusCode;

        _logger.LogInformation("Outgoing Response {Method} {Path} {StatusCode} in {Elapsed}ms TraceId: {TraceId}",
            method, path, statusCode, stopwatch.ElapsedMilliseconds, traceId);
    }
    
}
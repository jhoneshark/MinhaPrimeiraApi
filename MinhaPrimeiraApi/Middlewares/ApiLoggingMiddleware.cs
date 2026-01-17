using Hangfire;
using Microsoft.AspNetCore.Http.Extensions;
using MinhaPrimeiraApi.Domain.Interface;
using MinhaPrimeiraApi.Domain.Models;

namespace MinhaPrimeiraApi.Middlewares;

public class ApiLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IBackgroundJobClient _backgroundJobClient;
    
    private readonly string[] _ignoredPaths = { "/hangfire", "/swagger", "/favicon.ico" };

    public ApiLoggingMiddleware(RequestDelegate next, IBackgroundJobClient backgroundJobClient)
    {
       _next = next;
       _backgroundJobClient = backgroundJobClient;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();

        if (_ignoredPaths.Any(p => path != null && path.Contains(p)))
        {
            await _next(context);
            return;
        }
        
        // 2. Opcional: Logar apenas métodos de escrita (POST, PUT, DELETE, PATCH)
        // Se quiser logar GET também, basta remover esse IF
        /*
        if (context.Request.Method == HttpMethods.Get) {
            await _next(context);
            return;
        }
        */
        
        context.Request.EnableBuffering();
        var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0;
        
        var originalBodyStram = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;
        
        await _next(context);
        
        context.Response.Body.Position = 0;
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Position = 0;

        var log = new ApiResponseLog
        {
            Method = context.Request.Method,
            Url = context.Request.GetDisplayUrl(),
            RequestHeaders = System.Text.Json.JsonSerializer.Serialize(context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())),
            RequestBody = requestBody,
            Status = context.Response.StatusCode,
            ResponseHeaders = System.Text.Json.JsonSerializer.Serialize(context.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())),
            ResponseBody = responseBody,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        
        _backgroundJobClient.Enqueue<IApiLogService>(s => s.SaveLogAsync(log));
        
        await responseBodyStream.CopyToAsync(originalBodyStram);
    }

}
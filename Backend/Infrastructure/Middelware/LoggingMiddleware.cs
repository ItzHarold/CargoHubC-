using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Backend.Infrastructure.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        private static readonly Action<ILogger, string, string, string?, Exception?> _logExecutingAction;
        private static readonly Action<ILogger, string, string, string?, Exception?> _logExecutedAction;

        static LoggingMiddleware()
        {
            _logExecutingAction = LoggerMessage.Define<string, string, string?>(
                LogLevel.Information,
                new EventId(1, nameof(InvokeAsync)),
                "Executing action: {ActionName} with method: {Method}{Body}");

            _logExecutedAction = LoggerMessage.Define<string, string, string?>(
                LogLevel.Information,
                new EventId(2, nameof(InvokeAsync)),
                "Executed action: {ActionName} with method: {Method}{Body}");
        }

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;
            string? requestBody = null;

            if (request.Method != HttpMethods.Get)
            {
                requestBody = await ReadRequestBodyAsync(request);
            }

            // Extract API key from the headers
            var apiKey = context.Request.Headers["x-api-key"].FirstOrDefault() ?? "Unknown";

            _logExecutingAction(_logger, request.Path, request.Method, requestBody ?? string.Empty, null);

            // Resolve the scoped service
            using (var scope = context.RequestServices.CreateScope())
            {
                var logService = scope.ServiceProvider.GetRequiredService<Backend.Features.Logs.ILogService>();
                logService.LogRequest(apiKey, request.Method, requestBody);
            }

            await _next(context);

            _logExecutedAction(_logger, request.Path, request.Method, requestBody ?? string.Empty, null);
        }

        private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            request.EnableBuffering();
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return body;
        }
    }
}

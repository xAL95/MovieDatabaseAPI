using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MovieDatabaseAPI.Middleware
{
    public class ApiKey
    {
        private readonly RequestDelegate _next;
        private readonly string _apiKey;

        public ApiKey(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;

            // Get the API key from configuration
            _apiKey = configuration["ApiSettings:ApiKey"]!;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("key", out var extractedApiKey))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("API Key is missing.");
                return;
            }

            if (!string.Equals(_apiKey, extractedApiKey))
            {
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("Unauthorized client.");
                return;
            }
            
            await _next(context); // Call the next middleware in the pipeline
        }
    }
}

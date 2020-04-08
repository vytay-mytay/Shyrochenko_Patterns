using Microsoft.Extensions.DependencyInjection;
using ShyrochenkoPatterns.WebSockets.Managers;
using ShyrochenkoPatterns.WebSockets.Handlers;

namespace ShyrochenkoPatterns.WebSockets.Extentions
{
    public static class Extentions
    {
        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddTransient(typeof(WebSocketConnectionManager<>));

            services.AddSingleton(typeof(WebSocketMessageHandler));

            return services;
        }
    }
}

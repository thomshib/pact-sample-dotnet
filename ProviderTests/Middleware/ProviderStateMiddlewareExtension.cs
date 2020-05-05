using Microsoft.AspNetCore.Builder;
using ProviderTests.Middleware;


namespace ProviderTests.Middleware
{
// Extension method used to add the middleware to the HTTP request pipeline.
public static class ProviderStateMiddlewareExtension
{
    public static IApplicationBuilder UseProviderStateMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ProviderStateMiddleware>();
    }
} 

}
namespace TinyPath.WebApi.Middlewares;

public static class MiddlewareConfiguration
{
    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionResultMiddleware>();
        
        return app;
    }
}
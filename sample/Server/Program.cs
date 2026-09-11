using Microsoft.AspNetCore.SignalR;

var app = ServerFactory.Create(args);
await app.RunAsync();

public static class ServerFactory
{
    public static WebApplication Create(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
            .WithOrigins("http://127.0.0.1:5180", "http://localhost:5180")
            .AllowAnyHeader().AllowAnyMethod().AllowCredentials()));
        builder.Services.AddSignalR();
        var app = builder.Build();
        app.UseCors();
        // Supports optional hosting of published WASM assets; Client is built independently.
        app.UseBlazorFrameworkFiles();
        app.MapGet("/api/hello", () => "Hello from independent ASP.NET Core API");
        app.MapHub<EchoHub>("/hubs/echo");
        return app;
    }
}

public sealed class EchoHub : Hub
{
    public string Echo(string message) => message;
}

using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

public class ApiTests
{
    [Fact]
    public async Task ApiCorsAndSignalRWork()
    {
        await using var app = ServerFactory.Create(["--urls", "http://127.0.0.1:0"]);
        await app.StartAsync();
        var address = app.Urls.Single();
        using var http = new HttpClient { BaseAddress = new Uri(address) };
        Assert.Equal("Hello from independent ASP.NET Core API", await http.GetStringAsync("/api/hello"));
        using var request = new HttpRequestMessage(HttpMethod.Options, "/api/hello");
        request.Headers.Add("Origin", "http://127.0.0.1:5180");
        request.Headers.Add("Access-Control-Request-Method", "GET");
        using var response = await http.SendAsync(request);
        Assert.Contains("http://127.0.0.1:5180", response.Headers.GetValues("Access-Control-Allow-Origin"));
        await using var hub = new HubConnectionBuilder().WithUrl(address + "/hubs/echo").Build();
        await hub.StartAsync();
        Assert.Equal("round-trip verified", await hub.InvokeAsync<string>("Echo", "round-trip verified"));
        await app.StopAsync();
    }
}

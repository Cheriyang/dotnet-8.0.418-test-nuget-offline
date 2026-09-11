using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OfflineClient;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri("http://127.0.0.1:5181") });
await builder.Build().RunAsync();

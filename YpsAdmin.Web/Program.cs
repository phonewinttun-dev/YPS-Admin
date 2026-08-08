using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using YpsAdmin.Web;
using YpsAdmin.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var isHttps = builder.HostEnvironment.BaseAddress.StartsWith("https", StringComparison.OrdinalIgnoreCase);
var apiBaseUrl = isHttps
    ? (builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7119/")
    : (builder.Configuration["ApiBaseUrlHttp"] ?? "http://localhost:5214/");

var timeoutSeconds = int.TryParse(builder.Configuration["ApiTimeoutSeconds"], out int sec) ? sec : 30;

builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(apiBaseUrl),
    Timeout = TimeSpan.FromSeconds(timeoutSeconds)
});
builder.Services.AddScoped<LanguageService>();
builder.Services.AddScoped<IBusLineService, BusLineService>();
builder.Services.AddScoped<IBusStopService, BusStopService>();
builder.Services.AddScoped<IRouteStopService, RouteStopService>();
builder.Services.AddScoped<IYpsStoreService, YpsStoreService>();
builder.Services.AddScoped<ITownshipService, TownshipService>();

await builder.Build().RunAsync();

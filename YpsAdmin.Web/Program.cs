using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using YpsAdmin.Web;
using YpsAdmin.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] 
    ?? builder.Configuration["ApiBaseUrlHttp"] 
    ?? "http://localhost:5214/";

var timeoutSeconds = int.TryParse(builder.Configuration["ApiTimeoutSeconds"], out int sec) ? sec : 30;

builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(apiBaseUrl),
    Timeout = TimeSpan.FromSeconds(timeoutSeconds)
});
builder.Services.AddScoped<LanguageService>();
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IBusService, BusService>();
builder.Services.AddScoped<IBusStopService, BusStopService>();
builder.Services.AddScoped<IBusRouteService, BusRouteService>();
builder.Services.AddScoped<IRegionService, RegionService>();
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<IToastService, ToastService>();

await builder.Build().RunAsync();

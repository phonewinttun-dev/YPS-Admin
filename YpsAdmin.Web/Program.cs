using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using YpsAdmin.Web;
using YpsAdmin.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<LanguageService>();
builder.Services.AddScoped<IBusLineService, BusLineService>();
builder.Services.AddScoped<IBusStopService, BusStopService>();
builder.Services.AddScoped<IRouteStopService, RouteStopService>();
builder.Services.AddScoped<IYpsStoreService, YpsStoreService>();

await builder.Build().RunAsync();

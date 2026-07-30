using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YpsAdmin.Database.AppDbContextModels;
using YpsAdmin.Domain.Features.BusLine;
using YpsAdmin.Domain.Features.BusStop;
using YpsAdmin.Domain.Features.RouteStop;
using YpsAdmin.Domain.Features.Store;
using YpsAdmin.Domain.Features.Township;

namespace YpsAdmin.Domain.Features
{
    public static class FeatureManager
    {
        public static void AddDomain(this WebApplicationBuilder builder)
        {
            // Register DbContext with PostgreSQL & NetTopologySuite PostGIS support
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    o => o.UseNetTopologySuite()));

            // Register Feature Services
            builder.Services.AddScoped<IBusLineService, BusLineService>();
            builder.Services.AddScoped<IBusStopService, BusStopService>();
            builder.Services.AddScoped<IRouteStopService, RouteStopService>();
            builder.Services.AddScoped<IYpsStoreService, YpsStoreService>();
            builder.Services.AddScoped<ITownshipService, TownshipService>();
        }
    }
}

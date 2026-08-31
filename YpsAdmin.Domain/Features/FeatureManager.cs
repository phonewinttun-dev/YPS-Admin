using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YpsAdmin.Database.AppDbContextModels;
using YpsAdmin.Domain.Features.Bus;
using YpsAdmin.Domain.Features.BusRoute;
using YpsAdmin.Domain.Features.BusStop;
using YpsAdmin.Domain.Features.Dashboard;
using YpsAdmin.Domain.Features.Region;
using YpsAdmin.Domain.Features.Store;

namespace YpsAdmin.Domain.Features
{
    public static class FeatureManager
    {
        public static void AddDomain(this WebApplicationBuilder builder)
        {
            // Register DbContext with PostgreSQL
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    o =>
                    {
                        o.CommandTimeout(30);
                    }));

            // Register MemoryCache
            builder.Services.AddMemoryCache();

            // Register Feature Services
            builder.Services.AddScoped<IBusService, BusService>();
            builder.Services.AddScoped<IBusStopService, BusStopService>();
            builder.Services.AddScoped<IBusRouteService, BusRouteService>();
            builder.Services.AddScoped<IRegionService, RegionService>();
            builder.Services.AddScoped<IStoreService, StoreService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
        }
    }
}

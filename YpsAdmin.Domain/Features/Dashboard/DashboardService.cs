using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YpsAdmin.Database.AppDbContextModels;
using YpsAdmin.Domain.DTOs.Dashboard;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.Dashboard
{
 
    public sealed class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;
        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<DashboardSummaryDto>> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
        {
            var totalBusLines = await _context.TblBuses
                .AsNoTracking()
                .Where(b => b.DeleteFlag == false)
                .LongCountAsync(cancellationToken);

            var totalBusStops = await _context.TblBusStops
                .AsNoTracking()
                .Where(s => s.DeleteFlag == false)
                .LongCountAsync(cancellationToken);

            var totalStores = await _context.TblStores
                .AsNoTracking()
                .Where(st => st.DeleteFlag == false)
                .LongCountAsync(cancellationToken);

            var totalCardAcceptedBuses = await _context.TblBuses
                .AsNoTracking()
                .Where(b => b.DeleteFlag == false && b.IsCardAccepted == true)
                .LongCountAsync(cancellationToken);

            var totalRegions = await _context.TblRegions
                .AsNoTracking()
                .Where(r => r.DeleteFlag == false)
                .LongCountAsync(cancellationToken);

            var totalRouteMappings = await _context.TblBusRoutes
                .AsNoTracking()
                .LongCountAsync(cancellationToken);

            var summary = new DashboardSummaryDto(
                TotalBusLines: totalBusLines,
                TotalBusStops: totalBusStops,
                TotalStores: totalStores,
                TotalCardAcceptedBuses: totalCardAcceptedBuses,
                TotalRegions: totalRegions,
                TotalRouteMappings: totalRouteMappings);

            return Result<DashboardSummaryDto>.Success(summary, "Dashboard summary retrieved successfully.");
        }
    }
}

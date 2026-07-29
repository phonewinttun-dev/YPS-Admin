using Microsoft.EntityFrameworkCore;
using YpsAdmin.Database.AppDbContextModels;
using YpsAdmin.Domain.DTOs.RouteStop;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.RouteStop
{
    public class RouteStopService : IRouteStopService
    {
        private readonly AppDbContext _context;

        public RouteStopService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<FullRouteResponseDto>> GetFullRouteAsync(string routeId)
        {
            // Find bus line
            var busLine = await _context.Tblbuslines
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.RouteId == routeId);

            if (busLine == null)
            {
                return Result<FullRouteResponseDto>.Failure($"Bus line with Route ID '{routeId}' was not found.");
            }

            // Get all route stops for this bus line including Stop details
            var routeStops = await _context.Tblroutestops
                .AsNoTracking()
                .Include(rs => rs.Stop)
                .Where(rs => rs.RouteId == routeId)
                .ToListAsync();

            // Map outbound stops sorted by stop_order
            var outboundStops = routeStops
                .Where(rs => rs.Direction.Equals("Outbound", StringComparison.OrdinalIgnoreCase))
                .OrderBy(rs => rs.StopOrder)
                .Select(rs => new RouteStopDto
                {
                    Id = rs.Id,
                    RouteId = rs.RouteId,
                    StopId = rs.StopId,
                    Direction = rs.Direction,
                    StopOrder = rs.StopOrder,
                    StopType = rs.StopType,
                    StopNameMm = rs.Stop?.NameMm,
                    StopNameEn = rs.Stop?.NameEn,
                    TownshipMm = rs.Stop?.TownshipMm,
                    TownshipEn = rs.Stop?.TownshipEn,
                    RoadMm = rs.Stop?.RoadMm,
                    RoadEn = rs.Stop?.RoadEn
                })
                .ToList();

            // Map return stops sorted by stop_order
            var returnStops = routeStops
                .Where(rs => rs.Direction.Equals("Return", StringComparison.OrdinalIgnoreCase))
                .OrderBy(rs => rs.StopOrder)
                .Select(rs => new RouteStopDto
                {
                    Id = rs.Id,
                    RouteId = rs.RouteId,
                    StopId = rs.StopId,
                    Direction = rs.Direction,
                    StopOrder = rs.StopOrder,
                    StopType = rs.StopType,
                    StopNameMm = rs.Stop?.NameMm,
                    StopNameEn = rs.Stop?.NameEn,
                    TownshipMm = rs.Stop?.TownshipMm,
                    TownshipEn = rs.Stop?.TownshipEn,
                    RoadMm = rs.Stop?.RoadMm,
                    RoadEn = rs.Stop?.RoadEn
                })
                .ToList();

            var result = new FullRouteResponseDto
            {
                RouteId = busLine.RouteId,
                BusNumber = busLine.BusNumber,
                OutboundTitleMm = busLine.OutboundTitleMm,
                OutboundTitleEn = busLine.OutboundTitleEn,
                ReturnTitleMm = busLine.ReturnTitleMm,
                ReturnTitleEn = busLine.ReturnTitleEn,
                OutboundStops = outboundStops,
                ReturnStops = returnStops
            };

            return Result<FullRouteResponseDto>.Success(result, "Full route retrieved successfully.");
        }

        public async Task<Result<bool>> AssignStopsToRouteAsync(AssignRouteStopsRequest request)
        {
            // Validate Route ID
            var busLineExists = await _context.Tblbuslines.AnyAsync(b => b.RouteId == request.RouteId);
            if (!busLineExists)
            {
                return Result<bool>.Failure($"Bus line with Route ID '{request.RouteId}' was not found.");
            }

            if (request.Stops == null || request.Stops.Count == 0)
            {
                return Result<bool>.Failure("At least one stop must be provided.");
            }

            // Create new route stop entities
            foreach (var item in request.Stops)
            {
                // Verify stop exists if StopId is provided
                if (!string.IsNullOrWhiteSpace(item.StopId))
                {
                    bool stopExists = await _context.Tblbusstops.AnyAsync(s => s.StopId == item.StopId);
                    if (!stopExists)
                    {
                        return Result<bool>.Failure($"Bus stop with Stop ID '{item.StopId}' was not found.");
                    }
                }

                var routeStop = new Tblroutestop
                {
                    RouteId = request.RouteId,
                    StopId = item.StopId,
                    Direction = string.IsNullOrWhiteSpace(item.Direction) ? "Outbound" : item.Direction.Trim(),
                    StopOrder = item.StopOrder,
                    StopType = item.StopType
                };

                _context.Tblroutestops.Add(routeStop);
            }

            await _context.SaveChangesAsync();
            return Result<bool>.Success(true, "Stops assigned to bus line successfully.");
        }

        public async Task<Result<bool>> ReorderRouteStopsAsync(ReorderRouteStopsRequest request)
        {
            if (request.Items == null || request.Items.Count == 0)
            {
                return Result<bool>.Failure("No items provided for reordering.");
            }

            foreach (var item in request.Items)
            {
                var routeStop = await _context.Tblroutestops.FirstOrDefaultAsync(rs => rs.Id == item.RouteStopId);
                if (routeStop != null)
                {
                    routeStop.StopOrder = item.NewStopOrder;
                }
            }

            await _context.SaveChangesAsync();
            return Result<bool>.Success(true, "Route stops reordered successfully.");
        }

        public async Task<Result<bool>> RemoveRouteStopAsync(int routeStopId)
        {
            var routeStop = await _context.Tblroutestops.FirstOrDefaultAsync(rs => rs.Id == routeStopId);
            if (routeStop == null)
            {
                return Result<bool>.Failure($"Route stop record with ID {routeStopId} was not found.");
            }

            _context.Tblroutestops.Remove(routeStop);
            await _context.SaveChangesAsync();

            return Result<bool>.Success(true, "Route stop removed successfully.");
        }
    }
}

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

        public async Task<Result<FullRouteResponseDto>> GetFullRouteAsync(int routeId)
        {
            // Find bus line
            var busLine = await _context.TblBusLines
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.RouteId == routeId);

            if (busLine == null)
            {
                return Result<FullRouteResponseDto>.Failure($"Bus line with Route ID '{routeId}' was not found.");
            }

            // Get all route stops for this bus line including Stop and Township details
            var routeStops = await _context.TblRouteStops
                .AsNoTracking()
                .Include(rs => rs.Stop)
                    .ThenInclude(s => s!.Township)
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
                    TownshipId = rs.Stop?.TownshipId,
                    TownshipNameMm = rs.Stop?.Township?.TownshipNameMm,
                    TownshipNameEn = rs.Stop?.Township?.TownshipNameEn,
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
                    TownshipId = rs.Stop?.TownshipId,
                    TownshipNameMm = rs.Stop?.Township?.TownshipNameMm,
                    TownshipNameEn = rs.Stop?.Township?.TownshipNameEn,
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
            var busLineExists = await _context.TblBusLines.AnyAsync(b => b.RouteId == request.RouteId);
            if (!busLineExists)
            {
                return Result<bool>.Failure($"Bus line with Route ID '{request.RouteId}' was not found.");
            }

            if (request.Stops == null)
            {
                request.Stops = new List<AssignRouteStopItem>();
            }

            string direction = request.Stops.FirstOrDefault()?.Direction ?? "Outbound";

            // Remove existing stops for this route and direction to prevent duplicate order / key conflicts
            var existingStops = await _context.TblRouteStops
                .Where(rs => rs.RouteId == request.RouteId && rs.Direction.ToLower() == direction.ToLower())
                .ToListAsync();
            _context.TblRouteStops.RemoveRange(existingStops);

            int order = 1;
            foreach (var item in request.Stops)
            {
                if (item.StopId.HasValue)
                {
                    bool stopExists = await _context.TblBusStops.AnyAsync(s => s.StopId == item.StopId.Value);
                    if (!stopExists)
                    {
                        return Result<bool>.Failure($"Bus stop with Stop ID '{item.StopId.Value}' was not found.");
                    }
                }

                var routeStop = new TblRouteStop
                {
                    RouteId = request.RouteId,
                    StopId = item.StopId,
                    Direction = string.IsNullOrWhiteSpace(item.Direction) ? direction : item.Direction.Trim(),
                    StopOrder = item.StopOrder > 0 ? item.StopOrder : order,
                    StopType = item.StopType
                };

                _context.TblRouteStops.Add(routeStop);
                order++;
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
                var routeStop = await _context.TblRouteStops.FirstOrDefaultAsync(rs => rs.Id == item.RouteStopId);
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
            var routeStop = await _context.TblRouteStops.FirstOrDefaultAsync(rs => rs.Id == routeStopId);
            if (routeStop == null)
            {
                return Result<bool>.Failure($"Route stop record with ID {routeStopId} was not found.");
            }

            _context.TblRouteStops.Remove(routeStop);
            await _context.SaveChangesAsync();

            return Result<bool>.Success(true, "Route stop removed successfully.");
        }
    }
}

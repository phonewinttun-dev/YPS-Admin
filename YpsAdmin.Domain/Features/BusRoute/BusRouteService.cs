using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YpsAdmin.Database.AppDbContextModels;
using YpsAdmin.Domain.DTOs.BusRoute;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.BusRoute
{
    public class BusRouteService : IBusRouteService
    {
        private readonly AppDbContext _context;

        public BusRouteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<FullRouteResponseDto>> GetFullRouteAsync(long busId, CancellationToken cancellationToken = default)
        {
            var bus = await _context.TblBuses
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == busId, cancellationToken);

            if (bus == null)
            {
                return Result<FullRouteResponseDto>.Failure($"Bus with ID '{busId}' was not found.");
            }

            var routes = await _context.TblBusRoutes
                .AsNoTracking()
                .Include(br => br.BusStop)
                .ThenInclude(s => s.Region)
                .Where(br => br.BusId == busId)
                .OrderBy(br => br.StopOrder)
                .Select(br => new BusRouteStopItemDto
                {
                    BusId = br.BusId,
                    BusStopId = br.BusStopId,
                    StopOrder = br.StopOrder,
                    StopName = br.BusStop.StopName,
                    Lat = br.BusStop.Lat,
                    Lon = br.BusStop.Lon,
                    RegionId = br.BusStop.RegionId,
                    RegionName = br.BusStop.Region != null ? br.BusStop.Region.RegionName : null
                })
                .ToListAsync(cancellationToken);

            var result = new FullRouteResponseDto
            {
                BusId = bus.Id,
                BusNumber = bus.BusNumber,
                VariantId = bus.VariantId,
                IsCardAccepted = bus.IsCardAccepted ?? false,
                IsReversed = bus.IsReversed ?? false,
                Stops = routes
            };

            return Result<FullRouteResponseDto>.Success(result, "Full route retrieved successfully.");
        }

        public async Task<Result<bool>> AssignStopsToRouteAsync(AssignBusRoutesRequest request, CancellationToken cancellationToken = default)
        {
            var busExists = await _context.TblBuses.AnyAsync(b => b.Id == request.BusId, cancellationToken);
            if (!busExists)
            {
                return Result<bool>.Failure($"Bus with ID '{request.BusId}' was not found.");
            }

            if (request.Stops == null)
            {
                request.Stops = new List<AssignBusRouteStopItem>();
            }

            var requestedStopIds = request.Stops
                .Where(s => s.BusStopId > 0)
                .Select(s => s.BusStopId)
                .Distinct()
                .ToList();

            if (requestedStopIds.Count > 0)
            {
                var existingStopIds = await _context.TblBusStops
                    .Where(s => requestedStopIds.Contains(s.Id))
                    .Select(s => s.Id)
                    .ToListAsync(cancellationToken);

                var missingStopId = requestedStopIds.FirstOrDefault(id => !existingStopIds.Contains(id));
                if (missingStopId > 0)
                {
                    return Result<bool>.Failure($"Bus stop with ID '{missingStopId}' was not found.");
                }
            }

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var existingRoutes = await _context.TblBusRoutes
                    .Where(br => br.BusId == request.BusId)
                    .ToListAsync(cancellationToken);

                if (existingRoutes.Count > 0)
                {
                    _context.TblBusRoutes.RemoveRange(existingRoutes);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                int order = 1;
                foreach (var item in request.Stops)
                {
                    var busRoute = new TblBusRoute
                    {
                        BusId = request.BusId,
                        BusStopId = item.BusStopId,
                        StopOrder = item.StopOrder > 0 ? item.StopOrder : order
                    };

                    _context.TblBusRoutes.Add(busRoute);
                    order++;
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return Result<bool>.Success(true, "Stops assigned to bus route successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<bool>.Failure($"Failed to assign stops: {ex.Message}");
            }
        }

        public async Task<Result<bool>> ReorderRouteStopsAsync(ReorderBusRoutesRequest request, CancellationToken cancellationToken = default)
        {
            if (request.Items == null || request.Items.Count == 0)
            {
                return Result<bool>.Failure("No items provided for reordering.");
            }

            var existingRoutes = await _context.TblBusRoutes
                .Where(br => br.BusId == request.BusId)
                .ToListAsync(cancellationToken);

            if (existingRoutes.Count == 0)
            {
                return Result<bool>.Failure("No matching bus route stops found.");
            }

            var itemDict = request.Items.ToDictionary(i => i.BusStopId, i => i.NewStopOrder);

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Pass 1: Set temporary negative stop_order to break composite key (bus_id, stop_order) uniqueness collision
                int tempCounter = -1;
                foreach (var route in existingRoutes)
                {
                    if (itemDict.ContainsKey(route.BusStopId))
                    {
                        route.StopOrder = tempCounter--;
                    }
                }
                await _context.SaveChangesAsync(cancellationToken);

                // Pass 2: Set target new stop_order
                foreach (var route in existingRoutes)
                {
                    if (itemDict.TryGetValue(route.BusStopId, out int newOrder))
                    {
                        route.StopOrder = newOrder;
                    }
                }
                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
                return Result<bool>.Success(true, "Bus route stops reordered successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<bool>.Failure($"Failed to reorder route stops: {ex.Message}");
            }
        }

        public async Task<Result<bool>> RemoveRouteStopAsync(long busId, int stopOrder, CancellationToken cancellationToken = default)
        {
            var routeStop = await _context.TblBusRoutes
                .FirstOrDefaultAsync(br => br.BusId == busId && br.StopOrder == stopOrder, cancellationToken);

            if (routeStop == null)
            {
                return Result<bool>.Failure($"Bus route stop for Bus ID '{busId}' and Order '{stopOrder}' was not found.");
            }

            _context.TblBusRoutes.Remove(routeStop);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true, "Bus route stop removed successfully.");
        }
    }
}

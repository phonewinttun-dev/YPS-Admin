using Microsoft.EntityFrameworkCore;
using YpsAdmin.Database.AppDbContextModels;
using YpsAdmin.Domain.DTOs.BusStop;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.BusStop
{
    public class BusStopService : IBusStopService
    {
        private readonly AppDbContext _context;

        public BusStopService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<BusStopDto>> GetBusStopsAsync(BusStopQueryFilter filter)
        {
            var query = _context.Tblbusstops.AsNoTracking().AsQueryable();

            // Search by stop name (MM or EN) if search term is provided
            if (!string.IsNullOrWhiteSpace(filter.SearchStopName))
            {
                string search = filter.SearchStopName.Trim().ToLower();
                query = query.Where(s =>
                    s.NameMm.ToLower().Contains(search) ||
                    (s.NameEn != null && s.NameEn.ToLower().Contains(search)));
            }

            int totalCount = await query.CountAsync();

            int skip = (filter.PageNumber - 1) * filter.PageSize;

            var items = await query
                .OrderBy(s => s.StopId)
                .Skip(skip)
                .Take(filter.PageSize)
                .Select(s => new BusStopDto
                {
                    StopId = s.StopId,
                    NameMm = s.NameMm,
                    NameEn = s.NameEn,
                    TownshipMm = s.TownshipMm,
                    TownshipEn = s.TownshipEn,
                    RoadMm = s.RoadMm,
                    RoadEn = s.RoadEn,
                    TotalServingBusLines = s.TotalServingBusLines ?? 0
                })
                .ToListAsync();

            var pagination = new Pagination(filter.PageNumber, filter.PageSize, totalCount);
            return PagedResult<BusStopDto>.Success(items, pagination, "Bus stops retrieved successfully.");
        }

        public async Task<Result<BusStopDto>> GetBusStopByIdAsync(string stopId)
        {
            var busStop = await _context.Tblbusstops
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.StopId == stopId);

            if (busStop == null)
            {
                return Result<BusStopDto>.Failure($"Bus stop with Stop ID '{stopId}' was not found.");
            }

            var dto = new BusStopDto
            {
                StopId = busStop.StopId,
                NameMm = busStop.NameMm,
                NameEn = busStop.NameEn,
                TownshipMm = busStop.TownshipMm,
                TownshipEn = busStop.TownshipEn,
                RoadMm = busStop.RoadMm,
                RoadEn = busStop.RoadEn,
                TotalServingBusLines = busStop.TotalServingBusLines ?? 0
            };

            return Result<BusStopDto>.Success(dto, "Bus stop retrieved successfully.");
        }

        public async Task<Result<BusStopDto>> CreateBusStopAsync(CreateBusStopRequest request)
        {
            // Validate Stop ID
            if (string.IsNullOrWhiteSpace(request.StopId))
            {
                return Result<BusStopDto>.Failure("Stop ID is required.");
            }

            // Check for duplicate Stop ID (US-06 acceptance criterion)
            bool exists = await _context.Tblbusstops.AnyAsync(s => s.StopId == request.StopId);
            if (exists)
            {
                return Result<BusStopDto>.Failure($"A bus stop with Stop ID '{request.StopId}' already exists.");
            }

            var busStop = new Tblbusstop
            {
                StopId = request.StopId.Trim(),
                NameMm = request.NameMm?.Trim() ?? string.Empty,
                NameEn = request.NameEn?.Trim(),
                TownshipMm = request.TownshipMm?.Trim(),
                TownshipEn = request.TownshipEn?.Trim(),
                RoadMm = request.RoadMm?.Trim(),
                RoadEn = request.RoadEn?.Trim(),
                TotalServingBusLines = 0
            };

            _context.Tblbusstops.Add(busStop);
            await _context.SaveChangesAsync();

            var dto = new BusStopDto
            {
                StopId = busStop.StopId,
                NameMm = busStop.NameMm,
                NameEn = busStop.NameEn,
                TownshipMm = busStop.TownshipMm,
                TownshipEn = busStop.TownshipEn,
                RoadMm = busStop.RoadMm,
                RoadEn = busStop.RoadEn,
                TotalServingBusLines = 0
            };

            return Result<BusStopDto>.Success(dto, "Bus stop created successfully.");
        }

        public async Task<Result<BusStopDto>> UpdateBusStopAsync(string stopId, UpdateBusStopRequest request)
        {
            var busStop = await _context.Tblbusstops.FirstOrDefaultAsync(s => s.StopId == stopId);
            if (busStop == null)
            {
                return Result<BusStopDto>.Failure($"Bus stop with Stop ID '{stopId}' was not found.");
            }

            busStop.NameMm = request.NameMm?.Trim() ?? busStop.NameMm;
            busStop.NameEn = request.NameEn?.Trim();
            busStop.TownshipMm = request.TownshipMm?.Trim();
            busStop.TownshipEn = request.TownshipEn?.Trim();
            busStop.RoadMm = request.RoadMm?.Trim();
            busStop.RoadEn = request.RoadEn?.Trim();

            await _context.SaveChangesAsync();

            var dto = new BusStopDto
            {
                StopId = busStop.StopId,
                NameMm = busStop.NameMm,
                NameEn = busStop.NameEn,
                TownshipMm = busStop.TownshipMm,
                TownshipEn = busStop.TownshipEn,
                RoadMm = busStop.RoadMm,
                RoadEn = busStop.RoadEn,
                TotalServingBusLines = busStop.TotalServingBusLines ?? 0
            };

            return Result<BusStopDto>.Success(dto, "Bus stop updated successfully.");
        }

        public async Task<Result<bool>> DeleteBusStopAsync(string stopId)
        {
            var busStop = await _context.Tblbusstops.FirstOrDefaultAsync(s => s.StopId == stopId);
            if (busStop == null)
            {
                return Result<bool>.Failure($"Bus stop with Stop ID '{stopId}' was not found.");
            }

            _context.Tblbusstops.Remove(busStop);
            await _context.SaveChangesAsync();

            return Result<bool>.Success(true, "Bus stop deleted successfully.");
        }
    }
}

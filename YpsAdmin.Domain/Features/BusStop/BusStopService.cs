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
            var query = _context.TblBusStops.AsNoTracking().Include(s => s.Township).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchStopName))
            {
                string search = filter.SearchStopName.Trim().ToLower();
                query = query.Where(s =>
                    s.NameMm.ToLower().Contains(search) ||
                    (s.NameEn != null && s.NameEn.ToLower().Contains(search)));
            }

            if (filter.TownshipId.HasValue)
            {
                query = query.Where(s => s.TownshipId == filter.TownshipId.Value);
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
                    TownshipId = s.TownshipId,
                    TownshipNameMm = s.Township != null ? s.Township.TownshipNameMm : null,
                    TownshipNameEn = s.Township != null ? s.Township.TownshipNameEn : null,
                    RoadMm = s.RoadMm,
                    RoadEn = s.RoadEn,
                    TotalServingBusLines = s.TotalServingBusLines ?? 0
                })
                .ToListAsync();

            var pagination = new Pagination(filter.PageNumber, filter.PageSize, totalCount);
            return PagedResult<BusStopDto>.Success(items, pagination, "Bus stops retrieved successfully.");
        }

        public async Task<Result<BusStopDto>> GetBusStopByIdAsync(int stopId)
        {
            var busStop = await _context.TblBusStops
                .AsNoTracking()
                .Include(s => s.Township)
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
                TownshipId = busStop.TownshipId,
                TownshipNameMm = busStop.Township?.TownshipNameMm,
                TownshipNameEn = busStop.Township?.TownshipNameEn,
                RoadMm = busStop.RoadMm,
                RoadEn = busStop.RoadEn,
                TotalServingBusLines = busStop.TotalServingBusLines ?? 0
            };

            return Result<BusStopDto>.Success(dto, "Bus stop retrieved successfully.");
        }

        public async Task<Result<BusStopDto>> CreateBusStopAsync(CreateBusStopRequest request)
        {
            var busStop = new TblBusStop
            {
                NameMm = request.NameMm?.Trim() ?? string.Empty,
                NameEn = request.NameEn?.Trim(),
                TownshipId = request.TownshipId,
                RoadMm = request.RoadMm?.Trim(),
                RoadEn = request.RoadEn?.Trim(),
                TotalServingBusLines = 0
            };

            if (request.StopId.HasValue && request.StopId.Value > 0)
            {
                bool exists = await _context.TblBusStops.AnyAsync(s => s.StopId == request.StopId.Value);
                if (exists)
                {
                    return Result<BusStopDto>.Failure($"A bus stop with Stop ID '{request.StopId.Value}' already exists.");
                }
                busStop.StopId = request.StopId.Value;
            }

            _context.TblBusStops.Add(busStop);
            await _context.SaveChangesAsync();

            // Load Township details if applicable
            string? townshipNameMm = null;
            string? townshipNameEn = null;
            if (busStop.TownshipId.HasValue)
            {
                var township = await _context.TblTownships.FindAsync(busStop.TownshipId.Value);
                townshipNameMm = township?.TownshipNameMm;
                townshipNameEn = township?.TownshipNameEn;
            }

            var dto = new BusStopDto
            {
                StopId = busStop.StopId,
                NameMm = busStop.NameMm,
                NameEn = busStop.NameEn,
                TownshipId = busStop.TownshipId,
                TownshipNameMm = townshipNameMm,
                TownshipNameEn = townshipNameEn,
                RoadMm = busStop.RoadMm,
                RoadEn = busStop.RoadEn,
                TotalServingBusLines = 0
            };

            return Result<BusStopDto>.Success(dto, "Bus stop created successfully.");
        }

        public async Task<Result<BusStopDto>> UpdateBusStopAsync(int stopId, UpdateBusStopRequest request)
        {
            var busStop = await _context.TblBusStops
                .Include(s => s.Township)
                .FirstOrDefaultAsync(s => s.StopId == stopId);

            if (busStop == null)
            {
                return Result<BusStopDto>.Failure($"Bus stop with Stop ID '{stopId}' was not found.");
            }

            busStop.NameMm = request.NameMm?.Trim() ?? busStop.NameMm;
            busStop.NameEn = request.NameEn?.Trim();
            busStop.TownshipId = request.TownshipId;
            busStop.RoadMm = request.RoadMm?.Trim();
            busStop.RoadEn = request.RoadEn?.Trim();

            await _context.SaveChangesAsync();

            // Reload Township if changed
            string? townshipNameMm = null;
            string? townshipNameEn = null;
            if (busStop.TownshipId.HasValue)
            {
                var township = await _context.TblTownships.FindAsync(busStop.TownshipId.Value);
                townshipNameMm = township?.TownshipNameMm;
                townshipNameEn = township?.TownshipNameEn;
            }

            var dto = new BusStopDto
            {
                StopId = busStop.StopId,
                NameMm = busStop.NameMm,
                NameEn = busStop.NameEn,
                TownshipId = busStop.TownshipId,
                TownshipNameMm = townshipNameMm,
                TownshipNameEn = townshipNameEn,
                RoadMm = busStop.RoadMm,
                RoadEn = busStop.RoadEn,
                TotalServingBusLines = busStop.TotalServingBusLines ?? 0
            };

            return Result<BusStopDto>.Success(dto, "Bus stop updated successfully.");
        }

        public async Task<Result<bool>> DeleteBusStopAsync(int stopId)
        {
            var busStop = await _context.TblBusStops.FirstOrDefaultAsync(s => s.StopId == stopId);
            if (busStop == null)
            {
                return Result<bool>.Failure($"Bus stop with Stop ID '{stopId}' was not found.");
            }

            _context.TblBusStops.Remove(busStop);
            await _context.SaveChangesAsync();

            return Result<bool>.Success(true, "Bus stop deleted successfully.");
        }
    }
}

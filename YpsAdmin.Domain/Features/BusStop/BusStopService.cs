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

        public async Task<PagedResult<BusStopDto>> GetBusStopsAsync(BusStopGetRequest request, CancellationToken cancellationToken = default)
        {
            var baseQuery = _context.TblBusStops.AsNoTracking();

            if (request.TownshipId.HasValue)
            {
                baseQuery = baseQuery.Where(s => s.TownshipId == request.TownshipId.Value);
            }

            int totalCount = await baseQuery.CountAsync(cancellationToken);
            int skip = (request.PageNumber - 1) * request.PageSize;

            var items = await baseQuery
                .OrderBy(s => s.StopId)
                .Skip(skip)
                .Take(request.PageSize)
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
                .ToListAsync(cancellationToken);

            var pagination = new Pagination(request.PageNumber, request.PageSize, totalCount);
            return PagedResult<BusStopDto>.Success(items, pagination, "Bus stops retrieved successfully.");
        }

        public async Task<PagedResult<BusStopDto>> SearchBusStopsAsync(BusStopSearchRequest request, CancellationToken cancellationToken = default)
        {
            var baseQuery = _context.TblBusStops.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                string search = $"%{request.SearchTerm.Trim()}%";
                baseQuery = baseQuery.Where(s =>
                    EF.Functions.ILike(s.NameMm, search) ||
                    (s.NameEn != null && EF.Functions.ILike(s.NameEn, search)) ||
                    (s.RoadMm != null && EF.Functions.ILike(s.RoadMm, search)) ||
                    (s.RoadEn != null && EF.Functions.ILike(s.RoadEn, search)));
            }

            if (request.TownshipId.HasValue)
            {
                baseQuery = baseQuery.Where(s => s.TownshipId == request.TownshipId.Value);
            }

            int totalCount = await baseQuery.CountAsync(cancellationToken);
            int skip = (request.PageNumber - 1) * request.PageSize;

            var items = await baseQuery
                .OrderBy(s => s.StopId)
                .Skip(skip)
                .Take(request.PageSize)
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
                .ToListAsync(cancellationToken);

            var pagination = new Pagination(request.PageNumber, request.PageSize, totalCount);
            return PagedResult<BusStopDto>.Success(items, pagination, "Bus stop search completed successfully.");
        }

        public async Task<Result<BusStopDto>> GetBusStopByIdAsync(int stopId, CancellationToken cancellationToken = default)
        {
            var busStop = await _context.TblBusStops
                .AsNoTracking()
                .Include(s => s.Township)
                .FirstOrDefaultAsync(s => s.StopId == stopId, cancellationToken);

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

        public async Task<Result<BusStopDto>> CreateBusStopAsync(CreateBusStopRequest request, CancellationToken cancellationToken = default)
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
                bool exists = await _context.TblBusStops.AnyAsync(s => s.StopId == request.StopId.Value, cancellationToken);
                if (exists)
                {
                    return Result<BusStopDto>.Failure($"A bus stop with Stop ID '{request.StopId.Value}' already exists.");
                }
                busStop.StopId = request.StopId.Value;
            }

            _context.TblBusStops.Add(busStop);
            await _context.SaveChangesAsync(cancellationToken);

            // Load Township details if applicable
            string? townshipNameMm = null;
            string? townshipNameEn = null;
            if (busStop.TownshipId.HasValue)
            {
                var township = await _context.TblTownships.FindAsync(new object[] { busStop.TownshipId.Value }, cancellationToken);
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

        public async Task<Result<BusStopDto>> UpdateBusStopAsync(int stopId, UpdateBusStopRequest request, CancellationToken cancellationToken = default)
        {
            var busStop = await _context.TblBusStops
                .Include(s => s.Township)
                .FirstOrDefaultAsync(s => s.StopId == stopId, cancellationToken);

            if (busStop == null)
            {
                return Result<BusStopDto>.Failure($"Bus stop with Stop ID '{stopId}' was not found.");
            }

            busStop.NameMm = request.NameMm?.Trim() ?? busStop.NameMm;
            busStop.NameEn = request.NameEn?.Trim();
            busStop.TownshipId = request.TownshipId;
            busStop.RoadMm = request.RoadMm?.Trim();
            busStop.RoadEn = request.RoadEn?.Trim();

            await _context.SaveChangesAsync(cancellationToken);

            // Reload Township if changed
            string? townshipNameMm = null;
            string? townshipNameEn = null;
            if (busStop.TownshipId.HasValue)
            {
                var township = await _context.TblTownships.FindAsync(new object[] { busStop.TownshipId.Value }, cancellationToken);
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

        public async Task<Result<bool>> DeleteBusStopAsync(int stopId, CancellationToken cancellationToken = default)
        {
            var busStop = await _context.TblBusStops.FirstOrDefaultAsync(s => s.StopId == stopId, cancellationToken);
            if (busStop == null)
            {
                return Result<bool>.Failure($"Bus stop with Stop ID '{stopId}' was not found.");
            }

            _context.TblBusStops.Remove(busStop);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true, "Bus stop deleted successfully.");
        }
    }
}

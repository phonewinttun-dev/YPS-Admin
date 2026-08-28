using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            var query = _context.TblBusStops.AsNoTracking().Where(s => s.DeleteFlag != true);

            if (request.RegionId.HasValue)
            {
                query = query.Where(s => s.RegionId == request.RegionId.Value);
            }

            int totalCount = await query.CountAsync(cancellationToken);
            int skip = (request.PageNumber - 1) * request.PageSize;

            var items = await query
                .OrderBy(s => s.Id)
                .Skip(skip)
                .Take(request.PageSize)
                .Select(s => new BusStopDto
                {
                    Id = s.Id,
                    StopName = s.StopName,
                    Lat = s.Lat,
                    Lon = s.Lon,
                    RegionId = s.RegionId,
                    RegionName = s.Region != null ? s.Region.RegionName : null,
                    DeleteFlag = s.DeleteFlag ?? false,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToListAsync(cancellationToken);

            var pagination = new Pagination(request.PageNumber, request.PageSize, totalCount);
            return PagedResult<BusStopDto>.Success(items, pagination, "Bus stops retrieved successfully.");
        }

        public async Task<PagedResult<BusStopDto>> SearchBusStopsAsync(BusStopSearchRequest request, CancellationToken cancellationToken = default)
        {
            var query = _context.TblBusStops.AsNoTracking().Where(s => s.DeleteFlag != true);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                string search = $"%{request.SearchTerm.Trim()}%";
                query = query.Where(s => EF.Functions.ILike(s.StopName, search));
            }

            if (request.RegionId.HasValue)
            {
                query = query.Where(s => s.RegionId == request.RegionId.Value);
            }

            int totalCount = await query.CountAsync(cancellationToken);
            int skip = (request.PageNumber - 1) * request.PageSize;

            var items = await query
                .OrderBy(s => s.Id)
                .Skip(skip)
                .Take(request.PageSize)
                .Select(s => new BusStopDto
                {
                    Id = s.Id,
                    StopName = s.StopName,
                    Lat = s.Lat,
                    Lon = s.Lon,
                    RegionId = s.RegionId,
                    RegionName = s.Region != null ? s.Region.RegionName : null,
                    DeleteFlag = s.DeleteFlag ?? false,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToListAsync(cancellationToken);

            var pagination = new Pagination(request.PageNumber, request.PageSize, totalCount);
            return PagedResult<BusStopDto>.Success(items, pagination, "Bus stop search completed successfully.");
        }

        public async Task<Result<BusStopDto>> GetBusStopByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var busStop = await _context.TblBusStops
                .AsNoTracking()
                .Include(s => s.Region)
                .FirstOrDefaultAsync(s => s.Id == id && s.DeleteFlag != true, cancellationToken);

            if (busStop == null)
            {
                return Result<BusStopDto>.Failure($"Bus stop with ID '{id}' was not found.");
            }

            var dto = new BusStopDto
            {
                Id = busStop.Id,
                StopName = busStop.StopName,
                Lat = busStop.Lat,
                Lon = busStop.Lon,
                RegionId = busStop.RegionId,
                RegionName = busStop.Region?.RegionName,
                DeleteFlag = busStop.DeleteFlag ?? false,
                CreatedAt = busStop.CreatedAt,
                UpdatedAt = busStop.UpdatedAt
            };

            return Result<BusStopDto>.Success(dto, "Bus stop retrieved successfully.");
        }

        public async Task<Result<List<BusStopDto>>> GetBusStopByRegionAsync(int regionId, CancellationToken cancellationToken = default)
        {
            var busStops = await _context.TblBusStops
                .AsNoTracking()
                .Where(s => s.RegionId == regionId && s.DeleteFlag != true)
                .OrderBy(s => s.StopName)
                .Select(s => new BusStopDto
                {
                     Id = s.Id,
                     StopName = s.StopName,
                     Lat = s.Lat,
                     Lon = s.Lon,
                     RegionId = s.RegionId,
                     RegionName = s.Region != null ? s.Region.RegionName : null,
                     DeleteFlag = s.DeleteFlag ?? false,
                     CreatedAt = s.CreatedAt,
                     UpdatedAt = s.UpdatedAt
                })
                 .ToListAsync(cancellationToken);


            return Result<List<BusStopDto>>.Success(busStops, "Bus stop retrieved successfully.");
        }

        public async Task<Result<BusStopDto>> CreateBusStopAsync(CreateBusStopRequest request, CancellationToken cancellationToken = default)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var busStop = new TblBusStop
            {
                StopName = request.StopName.Trim(),
                Lat = request.Lat,
                Lon = request.Lon,
                RegionId = request.RegionId,
                DeleteFlag = false,
                CreatedAt = today,
                UpdatedAt = today
            };

            _context.TblBusStops.Add(busStop);
            await _context.SaveChangesAsync(cancellationToken);

            string? regionName = null;
            if (busStop.RegionId.HasValue)
            {
                var region = await _context.TblRegions.FindAsync(new object[] { busStop.RegionId.Value }, cancellationToken);
                regionName = region?.RegionName;
            }

            var dto = new BusStopDto
            {
                Id = busStop.Id,
                StopName = busStop.StopName,
                Lat = busStop.Lat,
                Lon = busStop.Lon,
                RegionId = busStop.RegionId,
                RegionName = regionName,
                DeleteFlag = false,
                CreatedAt = busStop.CreatedAt,
                UpdatedAt = busStop.UpdatedAt
            };

            return Result<BusStopDto>.Success(dto, "Bus stop created successfully.");
        }

        public async Task<Result<BusStopDto>> UpdateBusStopAsync(long id, UpdateBusStopRequest request, CancellationToken cancellationToken = default)
        {
            var busStop = await _context.TblBusStops
                .Include(s => s.Region)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (busStop == null)
            {
                return Result<BusStopDto>.Failure($"Bus stop with ID '{id}' was not found.");
            }

            busStop.StopName = request.StopName.Trim();
            busStop.Lat = request.Lat;
            busStop.Lon = request.Lon;
            busStop.RegionId = request.RegionId;
            if (request.DeleteFlag.HasValue)
            {
                busStop.DeleteFlag = request.DeleteFlag.Value;
            }
            busStop.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);

            await _context.SaveChangesAsync(cancellationToken);

            string? regionName = null;
            if (busStop.RegionId.HasValue)
            {
                var region = await _context.TblRegions.FindAsync(new object[] { busStop.RegionId.Value }, cancellationToken);
                regionName = region?.RegionName;
            }

            var dto = new BusStopDto
            {
                Id = busStop.Id,
                StopName = busStop.StopName,
                Lat = busStop.Lat,
                Lon = busStop.Lon,
                RegionId = busStop.RegionId,
                RegionName = regionName,
                DeleteFlag = busStop.DeleteFlag ?? false,
                CreatedAt = busStop.CreatedAt,
                UpdatedAt = busStop.UpdatedAt
            };

            return Result<BusStopDto>.Success(dto, "Bus stop updated successfully.");
        }

        public async Task<Result<bool>> DeleteBusStopAsync(long id, CancellationToken cancellationToken = default)
        {
            var busStop = await _context.TblBusStops.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
            if (busStop == null)
            {
                return Result<bool>.Failure($"Bus stop with ID '{id}' was not found.");
            }

            busStop.DeleteFlag = true;
            busStop.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true, "Bus stop deleted successfully.");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YpsAdmin.Database.AppDbContextModels;
using YpsAdmin.Domain.DTOs.Store;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.Store
{
    public class StoreService : IStoreService
    {
        private readonly AppDbContext _context;

        public StoreService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<StoreDto>> GetStoresAsync(StoreGetRequest request, CancellationToken cancellationToken = default)
        {
            var query = _context.TblStores.AsNoTracking().Where(s => s.DeleteFlag != true);

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
                .Select(s => new StoreDto
                {
                    Id = s.Id,
                    EngName = s.EngName,
                    MmName = s.MmName,
                    Category = s.Category,
                    Lat = s.Lat,
                    Lon = s.Lon,
                    RegionId = s.RegionId,
                    RegionName = s.Region != null ? s.Region.RegionName : null,
                    DeleteFlag = s.DeleteFlag ?? false,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    NearestStops = s.TblNearestBusStops.Select(ns => new NearestBusStopDto
                    {
                        Id = ns.Id,
                        BusStopId = ns.BusStopId,
                        StopName = ns.BusStop.StopName,
                        DistanceKm = ns.DistanceKm
                    }).ToList(),
                    ServingBusNumbers = s.TblNearestBusStops
                        .SelectMany(ns => ns.BusStop.TblBusRoutes.Select(br => br.Bus.BusNumber))
                        .Distinct()
                        .ToList()
                })
                .ToListAsync(cancellationToken);

            var pagination = new Pagination(request.PageNumber, request.PageSize, totalCount);
            return PagedResult<StoreDto>.Success(items, pagination, "Stores retrieved successfully.");
        }

        public async Task<PagedResult<StoreDto>> SearchStoresAsync(StoreSearchRequest request, CancellationToken cancellationToken = default)
        {
            var query = _context.TblStores.AsNoTracking().Where(s => s.DeleteFlag != true);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                string search = $"%{request.SearchTerm.Trim()}%";
                query = query.Where(s =>
                    (s.EngName != null && EF.Functions.ILike(s.EngName, search)) ||
                    (s.MmName != null && EF.Functions.ILike(s.MmName, search)) ||
                    (s.Category != null && EF.Functions.ILike(s.Category, search)) ||
                    (s.Region != null && EF.Functions.ILike(s.Region.RegionName, search)));
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
                .Select(s => new StoreDto
                {
                    Id = s.Id,
                    EngName = s.EngName,
                    MmName = s.MmName,
                    Category = s.Category,
                    Lat = s.Lat,
                    Lon = s.Lon,
                    RegionId = s.RegionId,
                    RegionName = s.Region != null ? s.Region.RegionName : null,
                    DeleteFlag = s.DeleteFlag ?? false,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    NearestStops = s.TblNearestBusStops.Select(ns => new NearestBusStopDto
                    {
                        Id = ns.Id,
                        BusStopId = ns.BusStopId,
                        StopName = ns.BusStop.StopName,
                        DistanceKm = ns.DistanceKm
                    }).ToList(),
                    ServingBusNumbers = s.TblNearestBusStops
                        .SelectMany(ns => ns.BusStop.TblBusRoutes.Select(br => br.Bus.BusNumber))
                        .Distinct()
                        .ToList()
                })
                .ToListAsync(cancellationToken);

            var pagination = new Pagination(request.PageNumber, request.PageSize, totalCount);
            return PagedResult<StoreDto>.Success(items, pagination, "Store search completed successfully.");
        }

        public async Task<Result<StoreDto>> GetStoreByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var store = await _context.TblStores
                .AsNoTracking()
                .Include(s => s.Region)
                .Include(s => s.TblNearestBusStops)
                    .ThenInclude(ns => ns.BusStop)
                        .ThenInclude(bs => bs.TblBusRoutes)
                            .ThenInclude(br => br.Bus)
                .AsSplitQuery()
                .FirstOrDefaultAsync(s => s.Id == id && s.DeleteFlag != true, cancellationToken);

            if (store == null)
            {
                return Result<StoreDto>.Failure($"Store with ID '{id}' was not found.");
            }

            var dto = new StoreDto
            {
                Id = store.Id,
                EngName = store.EngName,
                MmName = store.MmName,
                Category = store.Category,
                Lat = store.Lat,
                Lon = store.Lon,
                RegionId = store.RegionId,
                RegionName = store.Region?.RegionName,
                DeleteFlag = store.DeleteFlag ?? false,
                CreatedAt = store.CreatedAt,
                UpdatedAt = store.UpdatedAt,
                NearestStops = store.TblNearestBusStops.Select(ns => new NearestBusStopDto
                {
                    Id = ns.Id,
                    BusStopId = ns.BusStopId,
                    StopName = ns.BusStop?.StopName,
                    DistanceKm = ns.DistanceKm
                }).ToList(),
                ServingBusNumbers = store.TblNearestBusStops
                    .Where(ns => ns.BusStop != null)
                    .SelectMany(ns => ns.BusStop!.TblBusRoutes.Select(br => br.Bus.BusNumber))
                    .Distinct()
                    .ToList()
            };

            return Result<StoreDto>.Success(dto, "Store retrieved successfully.");
        }

        public async Task<Result<StoreDto>> CreateStoreAsync(CreateStoreRequest request, CancellationToken cancellationToken = default)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var store = new TblStore
            {
                EngName = request.EngName?.Trim(),
                MmName = request.MmName?.Trim(),
                Category = request.Category?.Trim(),
                Lat = request.Lat,
                Lon = request.Lon,
                RegionId = request.RegionId,
                DeleteFlag = false,
                CreatedAt = today,
                UpdatedAt = today
            };

            _context.TblStores.Add(store);
            await _context.SaveChangesAsync(cancellationToken);

            string? regionName = null;
            if (store.RegionId.HasValue)
            {
                var region = await _context.TblRegions.FindAsync(new object[] { store.RegionId.Value }, cancellationToken);
                regionName = region?.RegionName;
            }

            var dto = new StoreDto
            {
                Id = store.Id,
                EngName = store.EngName,
                MmName = store.MmName,
                Category = store.Category,
                Lat = store.Lat,
                Lon = store.Lon,
                RegionId = store.RegionId,
                RegionName = regionName,
                DeleteFlag = false,
                CreatedAt = store.CreatedAt,
                UpdatedAt = store.UpdatedAt
            };

            return Result<StoreDto>.Success(dto, "Store created successfully.");
        }

        public async Task<Result<StoreDto>> UpdateStoreAsync(int id, UpdateStoreRequest request, CancellationToken cancellationToken = default)
        {
            var store = await _context.TblStores
                .Include(s => s.Region)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (store == null)
            {
                return Result<StoreDto>.Failure($"Store with ID '{id}' was not found.");
            }

            if (request.EngName != null) store.EngName = request.EngName.Trim();
            if (request.MmName != null) store.MmName = request.MmName.Trim();
            if (request.Category != null) store.Category = request.Category.Trim();
            store.Lat = request.Lat;
            store.Lon = request.Lon;
            store.RegionId = request.RegionId;
            if (request.DeleteFlag.HasValue) store.DeleteFlag = request.DeleteFlag.Value;
            store.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);

            await _context.SaveChangesAsync(cancellationToken);

            string? regionName = null;
            if (store.RegionId.HasValue)
            {
                var region = await _context.TblRegions.FindAsync(new object[] { store.RegionId.Value }, cancellationToken);
                regionName = region?.RegionName;
            }

            var dto = new StoreDto
            {
                Id = store.Id,
                EngName = store.EngName,
                MmName = store.MmName,
                Category = store.Category,
                Lat = store.Lat,
                Lon = store.Lon,
                RegionId = store.RegionId,
                RegionName = regionName,
                DeleteFlag = store.DeleteFlag ?? false,
                CreatedAt = store.CreatedAt,
                UpdatedAt = store.UpdatedAt
            };

            return Result<StoreDto>.Success(dto, "Store updated successfully.");
        }

        public async Task<Result<bool>> DeleteStoreAsync(int id, CancellationToken cancellationToken = default)
        {
            var store = await _context.TblStores.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
            if (store == null)
            {
                return Result<bool>.Failure($"Store with ID '{id}' was not found.");
            }

            store.DeleteFlag = true;
            store.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true, "Store deleted successfully.");
        }

        public async Task<Result<bool>> AssignNearestStopsAsync(int storeId, AssignNearestStopsRequest request, CancellationToken cancellationToken = default)
        {
            var store = await _context.TblStores
                .Include(s => s.TblNearestBusStops)
                .FirstOrDefaultAsync(s => s.Id == storeId, cancellationToken);

            if (store == null)
            {
                return Result<bool>.Failure($"Store with ID '{storeId}' was not found.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                if (store.TblNearestBusStops.Count > 0)
                {
                    _context.TblNearestBusStops.RemoveRange(store.TblNearestBusStops);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                if (request.NearestStops != null && request.NearestStops.Count > 0)
                {
                    var distinctStops = request.NearestStops
                        .Where(ns => ns.BusStopId > 0)
                        .GroupBy(ns => ns.BusStopId)
                        .Select(g => g.First())
                        .ToList();

                    var today = DateOnly.FromDateTime(DateTime.UtcNow);
                    foreach (var item in distinctStops)
                    {
                        var nearestStop = new TblNearestBusStop
                        {
                            StoreId = storeId,
                            BusStopId = item.BusStopId,
                            DistanceKm = item.DistanceKm,
                            CreatedAt = today,
                            UpdatedAt = today
                        };
                        _context.TblNearestBusStops.Add(nearestStop);
                    }

                    await _context.SaveChangesAsync(cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
                return Result<bool>.Success(true, "Nearest bus stops assigned to store successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<bool>.Failure($"Failed to assign nearest stops: {ex.Message}");
            }
        }
    }
}

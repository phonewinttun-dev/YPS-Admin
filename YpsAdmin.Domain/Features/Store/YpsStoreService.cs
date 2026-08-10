using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using YpsAdmin.Database.AppDbContextModels;
using YpsAdmin.Domain.DTOs.Store;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.Store
{
    public class YpsStoreService : IYpsStoreService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public YpsStoreService(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<PagedResult<YpsStoreDto>> GetYpsStoresAsync(YpsStoreGetRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var cacheKey = "stores_all";
                var allStores = await _cache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    entry.SetAbsoluteExpiration(TimeSpan.FromHours(24));
                    entry.SetPriority(CacheItemPriority.High);

                    return await _context.TblYpsStores
                        .AsNoTracking()
                        .OrderBy(s => s.StoreId)
                        .Select(s => new YpsStoreDto
                        {
                            StoreId = s.StoreId,
                            NameMm = s.NameMm,
                            NameEn = s.NameEn,
                            Category = s.Category,
                            TownshipId = s.TownshipId,
                            TownshipNameMm = s.Township != null ? s.Township.TownshipNameMm : null,
                            TownshipNameEn = s.Township != null ? s.Township.TownshipNameEn : null,
                            Latitude = s.Latitude,
                            Longitude = s.Longitude,
                            NearestStops = s.TblYpsStoreNearestStops.Select(ns => new NearestStopDto
                            {
                                Id = ns.Id,
                                StopNameMm = ns.StopNameMm,
                                StopNameEn = ns.StopNameEn,
                                MatchedStopId = ns.MatchedStopId
                            }).ToList(),
                            ServingBusLines = s.TblYpsStoreServingBusLines
                                .Select(sl => sl.BusNumber)
                                .ToList()
                        })
                        .ToListAsync(cancellationToken);
                });

                var filteredStores = allStores ?? new List<YpsStoreDto>();

                if (request.TownshipId.HasValue)
                {
                    filteredStores = filteredStores.Where(s => s.TownshipId == request.TownshipId.Value).ToList();
                }

                int totalCount = filteredStores.Count;
                int skip = (request.PageNumber - 1) * request.PageSize;

                var pagedItems = filteredStores
                    .Skip(skip)
                    .Take(request.PageSize)
                    .ToList();

                var pagination = new Pagination(request.PageNumber, request.PageSize, totalCount);
                return PagedResult<YpsStoreDto>.Success(pagedItems, pagination, "YPS stores retrieved successfully.");
            }
            catch (Exception ex)
            {
                return PagedResult<YpsStoreDto>.Failure($"Failed to retrieve stores: {ex.Message}");
            }
        }

        public async Task<PagedResult<YpsStoreDto>> SearchYpsStoresAsync(YpsStoreSearchRequest request, CancellationToken cancellationToken = default)
        {
            var query = _context.TblYpsStores.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.TownshipName))
            {
                string search = $"%{request.TownshipName.Trim()}%";
                query = query.Where(s => s.Township != null &&
                    (EF.Functions.ILike(s.Township.TownshipNameMm, search) ||
                     (s.Township.TownshipNameEn != null && EF.Functions.ILike(s.Township.TownshipNameEn, search))));
            }

            int totalCount = await query.CountAsync(cancellationToken);
            int skip = (request.PageNumber - 1) * request.PageSize;

            var dtos = await query
                .OrderBy(s => s.StoreId)
                .Skip(skip)
                .Take(request.PageSize)
                .Select(s => new YpsStoreDto
                {
                    StoreId = s.StoreId,
                    NameMm = s.NameMm,
                    NameEn = s.NameEn,
                    Category = s.Category,
                    TownshipId = s.TownshipId,
                    TownshipNameMm = s.Township != null ? s.Township.TownshipNameMm : null,
                    TownshipNameEn = s.Township != null ? s.Township.TownshipNameEn : null,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    NearestStops = s.TblYpsStoreNearestStops.Select(ns => new NearestStopDto
                    {
                        Id = ns.Id,
                        StopNameMm = ns.StopNameMm,
                        StopNameEn = ns.StopNameEn,
                        MatchedStopId = ns.MatchedStopId
                    }).ToList(),
                    ServingBusLines = s.TblYpsStoreServingBusLines
                        .Select(sl => sl.BusNumber)
                        .ToList()
                })
                .ToListAsync(cancellationToken);

            var pagination = new Pagination(request.PageNumber, request.PageSize, totalCount);
            return PagedResult<YpsStoreDto>.Success(dtos, pagination, "YPS store search completed successfully.");
        }

        public async Task<Result<YpsStoreDto>> GetYpsStoreByIdAsync(int storeId, CancellationToken cancellationToken = default)
        {
            var store = await _context.TblYpsStores
                .AsNoTracking()
                .Include(s => s.Township)
                .Include(s => s.TblYpsStoreNearestStops)
                .Include(s => s.TblYpsStoreServingBusLines)
                .AsSplitQuery()
                .FirstOrDefaultAsync(s => s.StoreId == storeId, cancellationToken);

            if (store == null)
            {
                return Result<YpsStoreDto>.Failure($"YPS store with Store ID '{storeId}' was not found.");
            }

            var dto = new YpsStoreDto
            {
                StoreId = store.StoreId,
                NameMm = store.NameMm,
                NameEn = store.NameEn,
                Category = store.Category,
                TownshipId = store.TownshipId,
                TownshipNameMm = store.Township?.TownshipNameMm,
                TownshipNameEn = store.Township?.TownshipNameEn,
                Latitude = store.Latitude,
                Longitude = store.Longitude,
                NearestStops = store.TblYpsStoreNearestStops.Select(ns => new NearestStopDto
                {
                    Id = ns.Id,
                    StopNameMm = ns.StopNameMm,
                    StopNameEn = ns.StopNameEn,
                    MatchedStopId = ns.MatchedStopId
                }).ToList(),
                ServingBusLines = store.TblYpsStoreServingBusLines
                    .Select(sl => sl.BusNumber)
                    .ToList()
            };

            return Result<YpsStoreDto>.Success(dto, "YPS store retrieved successfully.");
        }

        public async Task<Result<YpsStoreDto>> CreateYpsStoreAsync(CreateYpsStoreRequest request, CancellationToken cancellationToken = default)
        {
            Point? geom = null;
            if (request.Latitude.HasValue && request.Longitude.HasValue)
            {
                geom = new Point((double)request.Longitude.Value, (double)request.Latitude.Value)
                {
                    SRID = 4326
                };
            }

            var store = new TblYpsStore
            {
                NameMm = request.NameMm?.Trim() ?? string.Empty,
                NameEn = request.NameEn?.Trim(),
                Category = request.Category?.Trim(),
                TownshipId = request.TownshipId,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Geom = geom
            };

            if (request.StoreId.HasValue && request.StoreId.Value > 0)
            {
                bool exists = await _context.TblYpsStores.AnyAsync(s => s.StoreId == request.StoreId.Value, cancellationToken);
                if (exists)
                {
                    return Result<YpsStoreDto>.Failure($"A YPS store with Store ID '{request.StoreId.Value}' already exists.");
                }
                store.StoreId = request.StoreId.Value;
            }

            _context.TblYpsStores.Add(store);
            await _context.SaveChangesAsync(cancellationToken);

            string? townshipNameMm = null;
            string? townshipNameEn = null;
            if (store.TownshipId.HasValue)
            {
                var township = await _context.TblTownships.FindAsync(new object[] { store.TownshipId.Value }, cancellationToken);
                townshipNameMm = township?.TownshipNameMm;
                townshipNameEn = township?.TownshipNameEn;
            }

            var dto = new YpsStoreDto
            {
                StoreId = store.StoreId,
                NameMm = store.NameMm,
                NameEn = store.NameEn,
                Category = store.Category,
                TownshipId = store.TownshipId,
                TownshipNameMm = townshipNameMm,
                TownshipNameEn = townshipNameEn,
                Latitude = store.Latitude,
                Longitude = store.Longitude
            };

            return Result<YpsStoreDto>.Success(dto, "YPS store created successfully.");
        }

        public async Task<Result<YpsStoreDto>> UpdateYpsStoreAsync(int storeId, UpdateYpsStoreRequest request, CancellationToken cancellationToken = default)
        {
            var store = await _context.TblYpsStores
                .Include(s => s.Township)
                .Include(s => s.TblYpsStoreNearestStops)
                .Include(s => s.TblYpsStoreServingBusLines)
                .FirstOrDefaultAsync(s => s.StoreId == storeId, cancellationToken);

            if (store == null)
            {
                return Result<YpsStoreDto>.Failure($"YPS store with Store ID '{storeId}' was not found.");
            }

            if (request.NameMm != null)
            {
                store.NameMm = request.NameMm.Trim();
            }

            if (request.NameEn != null)
            {
                store.NameEn = request.NameEn.Trim();
            }

            if (request.Category != null)
            {
                store.Category = request.Category.Trim();
            }

            if (request.TownshipId.HasValue)
            {
                store.TownshipId = request.TownshipId;
            }

            if (request.Latitude.HasValue)
            {
                store.Latitude = request.Latitude;
            }

            if (request.Longitude.HasValue)
            {
                store.Longitude = request.Longitude;
            }

            if (store.Latitude.HasValue && store.Longitude.HasValue)
            {
                store.Geom = new Point((double)store.Longitude.Value, (double)store.Latitude.Value)
                {
                    SRID = 4326
                };
            }
            else
            {
                store.Geom = null;
            }

            await _context.SaveChangesAsync(cancellationToken);

            string? townshipNameMm = null;
            string? townshipNameEn = null;
            if (store.TownshipId.HasValue)
            {
                var township = await _context.TblTownships.FindAsync(new object[] { store.TownshipId.Value }, cancellationToken);
                townshipNameMm = township?.TownshipNameMm;
                townshipNameEn = township?.TownshipNameEn;
            }

            var dto = new YpsStoreDto
            {
                StoreId = store.StoreId,
                NameMm = store.NameMm,
                NameEn = store.NameEn,
                Category = store.Category,
                TownshipId = store.TownshipId,
                TownshipNameMm = townshipNameMm,
                TownshipNameEn = townshipNameEn,
                Latitude = store.Latitude,
                Longitude = store.Longitude,
                NearestStops = store.TblYpsStoreNearestStops.Select(ns => new NearestStopDto
                {
                    Id = ns.Id,
                    StopNameMm = ns.StopNameMm,
                    StopNameEn = ns.StopNameEn,
                    MatchedStopId = ns.MatchedStopId
                }).ToList(),
                ServingBusLines = store.TblYpsStoreServingBusLines
                    .Select(sl => sl.BusNumber)
                    .ToList()
            };

            return Result<YpsStoreDto>.Success(dto, "YPS store updated successfully.");
        }

        public async Task<Result<bool>> DeleteYpsStoreAsync(int storeId, CancellationToken cancellationToken = default)
        {
            var store = await _context.TblYpsStores.FirstOrDefaultAsync(s => s.StoreId == storeId, cancellationToken);
            if (store == null)
            {
                return Result<bool>.Failure($"YPS store with Store ID '{storeId}' was not found.");
            }

            _context.TblYpsStores.Remove(store);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true, "YPS store deleted successfully.");
        }

        public async Task<Result<bool>> AssignNearestStopsAsync(int storeId, AssignNearestStopsRequest request, CancellationToken cancellationToken = default)
        {
            var store = await _context.TblYpsStores
                .Include(s => s.TblYpsStoreNearestStops)
                .FirstOrDefaultAsync(s => s.StoreId == storeId, cancellationToken);

            if (store == null)
            {
                return Result<bool>.Failure($"YPS store with Store ID '{storeId}' was not found.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                if (store.TblYpsStoreNearestStops.Count > 0)
                {
                    _context.TblYpsStoreNearestStops.RemoveRange(store.TblYpsStoreNearestStops);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                if (request.NearestStops != null && request.NearestStops.Count > 0)
                {
                    // Deduplicate input stops by MatchedStopId to prevent unique constraint UQ_YpsStore_Nearest violations
                    var distinctStops = request.NearestStops
                        .GroupBy(ns => ns.MatchedStopId)
                        .Select(g => g.First())
                        .ToList();

                    foreach (var item in distinctStops)
                    {
                        var nearestStop = new TblYpsStoreNearestStop
                        {
                            StoreId = storeId,
                            MatchedStopId = item.MatchedStopId,
                            StopNameMm = item.StopNameMm,
                            StopNameEn = item.StopNameEn
                        };
                        _context.TblYpsStoreNearestStops.Add(nearestStop);
                    }

                    await _context.SaveChangesAsync(cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
                return Result<bool>.Success(true, "Nearest bus stops assigned to YPS store successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<bool>.Failure($"Failed to assign nearest stops: {ex.Message}");
            }
        }

        public async Task<Result<bool>> AssignServingBusLinesAsync(int storeId, AssignServingBusLinesRequest request, CancellationToken cancellationToken = default)
        {
            var store = await _context.TblYpsStores
                .Include(s => s.TblYpsStoreServingBusLines)
                .FirstOrDefaultAsync(s => s.StoreId == storeId, cancellationToken);

            if (store == null)
            {
                return Result<bool>.Failure($"YPS store with Store ID '{storeId}' was not found.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                if (store.TblYpsStoreServingBusLines.Count > 0)
                {
                    _context.TblYpsStoreServingBusLines.RemoveRange(store.TblYpsStoreServingBusLines);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                if (request.BusNumbers != null && request.BusNumbers.Count > 0)
                {
                    var distinctBusNumbers = request.BusNumbers.Distinct().ToList();

                    // Batch fetch matching RouteIds for all requested bus numbers to avoid N+1 queries
                    var busLines = await _context.TblBusLines
                        .Where(b => distinctBusNumbers.Contains(b.BusNumber))
                        .ToListAsync(cancellationToken);

                    var busLineDict = busLines
                        .GroupBy(b => b.BusNumber)
                        .ToDictionary(g => g.Key, g => g.First().RouteId);

                    // Track added route IDs to enforce unique index constraint UQ_YpsStore_Serving (StoreId, RouteId)
                    var addedRouteIds = new HashSet<int>();

                    foreach (var busNumber in distinctBusNumbers)
                    {
                        busLineDict.TryGetValue(busNumber, out int routeIdVal);
                        int? routeId = routeIdVal > 0 ? routeIdVal : null;

                        if (routeId.HasValue && addedRouteIds.Contains(routeId.Value))
                        {
                            continue; // Skip duplicate RouteId mapping for this store
                        }

                        if (routeId.HasValue)
                        {
                            addedRouteIds.Add(routeId.Value);
                        }

                        var servingBusLine = new TblYpsStoreServingBusLine
                        {
                            StoreId = storeId,
                            BusNumber = busNumber,
                            RouteId = routeId
                        };
                        _context.TblYpsStoreServingBusLines.Add(servingBusLine);
                    }

                    await _context.SaveChangesAsync(cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
                return Result<bool>.Success(true, "Serving bus lines assigned to YPS store successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<bool>.Failure($"Failed to assign serving bus lines: {ex.Message}");
            }
        }
    }
}

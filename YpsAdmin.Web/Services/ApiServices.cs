using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using YpsAdmin.Shared;

namespace YpsAdmin.Web.Services;

// DTOs matching Backend API specs
public record BusDto(long Id, long BusNumber, string? VariantId, bool IsCardAccepted, bool IsReversed, bool DeleteFlag, DateOnly CreatedAt, DateOnly UpdatedAt);
public record CreateBusRequest(long BusNumber, string? VariantId, bool? IsCardAccepted, bool? IsReversed);
public record UpdateBusRequest(long BusNumber, string? VariantId, bool? IsCardAccepted, bool? IsReversed, bool? DeleteFlag);

public record BusStopDto(long Id, string StopName, double Lat, double Lon, int? RegionId, string? RegionName, bool DeleteFlag, DateOnly CreatedAt, DateOnly UpdatedAt);
public record CreateBusStopRequest(string StopName, double Lat, double Lon, int? RegionId);
public record UpdateBusStopRequest(string StopName, double Lat, double Lon, int? RegionId, bool? DeleteFlag);

public record BusRouteStopItemDto(long BusId, long BusStopId, int StopOrder, string StopName, double Lat, double Lon, int? RegionId, string? RegionName);
public record FullRouteResponseDto(long BusId, long BusNumber, string? VariantId, bool IsCardAccepted, bool IsReversed, List<BusRouteStopItemDto> Stops);
public record AssignBusRouteStopItem(long BusStopId, int StopOrder);
public record AssignBusRoutesRequest(long BusId, List<AssignBusRouteStopItem> Stops);
public record ReorderBusRouteItem(long BusStopId, int OldStopOrder, int NewStopOrder);
public record ReorderBusRoutesRequest(long BusId, List<ReorderBusRouteItem> Items);

public record RegionDto(int Id, string RegionName, bool DeleteFlag, DateOnly CreatedAt, DateOnly UpdatedAt);
public record CreateRegionRequest(string RegionName);
public record UpdateRegionRequest(string RegionName, bool? DeleteFlag);

public record NearestBusStopDto(long Id, long BusStopId, string? StopName, double? DistanceKm);
public record StoreDto(int Id, string? EngName, string? MmName, string? Category, double Lat, double Lon, int? RegionId, string? RegionName, bool DeleteFlag, DateOnly? CreatedAt, DateOnly? UpdatedAt, List<NearestBusStopDto> NearestStops, List<long> ServingBusNumbers);
public record CreateStoreRequest(string? EngName, string? MmName, string? Category, double Lat, double Lon, int? RegionId);
public record UpdateStoreRequest(string? EngName, string? MmName, string? Category, double Lat, double Lon, int? RegionId, bool? DeleteFlag);
public record AssignNearestBusStopItem(long BusStopId, double? DistanceKm);
public record AssignNearestStopsRequest(List<AssignNearestBusStopItem> NearestStops);

// Services Interfaces
public interface IBusService
{
    Task<PagedResult<BusDto>?> GetBusesAsync(int pageNumber, int pageSize, string? searchBusNumber, string? variantId = null);
    Task<Result<BusDto>?> GetByIdAsync(long id);
    Task<Result<BusDto>?> CreateAsync(CreateBusRequest request);
    Task<Result<BusDto>?> UpdateAsync(long id, UpdateBusRequest request);
    Task<Result<bool>?> DeleteAsync(long id);
}

public interface IBusStopService
{
    Task<PagedResult<BusStopDto>?> GetBusStopsAsync(int pageNumber, int pageSize, string? searchStopName, int? regionId = null);
    Task<Result<BusStopDto>?> GetByIdAsync(long id);
    Task<Result<BusStopDto>?> CreateAsync(CreateBusStopRequest request);
    Task<Result<BusStopDto>?> UpdateAsync(long id, UpdateBusStopRequest request);
    Task<Result<bool>?> DeleteAsync(long id);
}

public interface IBusRouteService
{
    Task<Result<FullRouteResponseDto>> GetFullRouteAsync(long busId);
    Task<Result<bool>> AssignRouteStopsAsync(AssignBusRoutesRequest request);
    Task<Result<bool>> ReorderRouteStopsAsync(ReorderBusRoutesRequest request);
    Task<Result<bool>> DeleteRouteStopAsync(long busId, int stopOrder);
}

public interface IRegionService
{
    Task<PagedResult<RegionDto>?> GetRegionsAsync(int pageNumber, int pageSize, string? searchName);
    Task<Result<RegionDto>?> GetByIdAsync(int id);
    Task<Result<RegionDto>?> CreateAsync(CreateRegionRequest request);
    Task<Result<RegionDto>?> UpdateAsync(int id, UpdateRegionRequest request);
    Task<Result<bool>?> DeleteAsync(int id);
}

public interface IStoreService
{
    Task<PagedResult<StoreDto>?> GetStoresAsync(int pageNumber, int pageSize, string? searchName, int? regionId = null);
    Task<Result<StoreDto>?> GetByIdAsync(int id);
    Task<Result<StoreDto>?> CreateAsync(CreateStoreRequest request);
    Task<Result<StoreDto>?> UpdateAsync(int id, UpdateStoreRequest request);
    Task<Result<bool>?> DeleteAsync(int id);
    Task<Result<bool>?> AssignNearestStopsAsync(int id, AssignNearestStopsRequest request);
}

// Service Implementations
public class BusService : IBusService
{
    private readonly HttpClient _http;
    public BusService(HttpClient http) => _http = http;

    public async Task<PagedResult<BusDto>?> GetBusesAsync(int pageNumber, int pageSize, string? searchBusNumber, string? variantId = null)
    {
        try
        {
            var url = !string.IsNullOrWhiteSpace(searchBusNumber) || !string.IsNullOrWhiteSpace(variantId)
                ? $"/api/buses/search?pageNumber={pageNumber}&pageSize={pageSize}&busNumber={Uri.EscapeDataString(searchBusNumber ?? "")}&variantId={Uri.EscapeDataString(variantId ?? "")}"
                : $"/api/buses?pageNumber={pageNumber}&pageSize={pageSize}";
            return await _http.GetFromJsonAsync<PagedResult<BusDto>>(url);
        }
        catch (TaskCanceledException)
        {
            return PagedResult<BusDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return PagedResult<BusDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<BusDto>?> GetByIdAsync(long id)
    {
        try
        {
            return await _http.GetFromJsonAsync<Result<BusDto>>($"/api/buses/{id}");
        }
        catch (TaskCanceledException)
        {
            return Result<BusDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<BusDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<BusDto>?> CreateAsync(CreateBusRequest request)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("/api/buses", request);
            return await resp.Content.ReadFromJsonAsync<Result<BusDto>>();
        }
        catch (TaskCanceledException)
        {
            return Result<BusDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<BusDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<BusDto>?> UpdateAsync(long id, UpdateBusRequest request)
    {
        try
        {
            var resp = await _http.PutAsJsonAsync($"/api/buses/{id}", request);
            return await resp.Content.ReadFromJsonAsync<Result<BusDto>>();
        }
        catch (TaskCanceledException)
        {
            return Result<BusDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<BusDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<bool>?> DeleteAsync(long id)
    {
        try
        {
            var resp = await _http.DeleteAsync($"/api/buses/{id}");
            return await resp.Content.ReadFromJsonAsync<Result<bool>>();
        }
        catch (TaskCanceledException)
        {
            return Result<bool>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<bool>.Failure($"API request failed: {ex.Message}");
        }
    }
}

public class BusStopService : IBusStopService
{
    private readonly HttpClient _http;
    public BusStopService(HttpClient http) => _http = http;

    public async Task<PagedResult<BusStopDto>?> GetBusStopsAsync(int pageNumber, int pageSize, string? searchStopName, int? regionId = null)
    {
        try
        {
            string url;
            if (!string.IsNullOrWhiteSpace(searchStopName))
            {
                url = $"/api/bus-stops/search?pageNumber={pageNumber}&pageSize={pageSize}&searchTerm={Uri.EscapeDataString(searchStopName)}";
                if (regionId.HasValue) url += $"&regionId={regionId.Value}";
            }
            else
            {
                url = $"/api/bus-stops?pageNumber={pageNumber}&pageSize={pageSize}";
                if (regionId.HasValue) url += $"&regionId={regionId.Value}";
            }
            return await _http.GetFromJsonAsync<PagedResult<BusStopDto>>(url);
        }
        catch (TaskCanceledException)
        {
            return PagedResult<BusStopDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return PagedResult<BusStopDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<BusStopDto>?> GetByIdAsync(long id)
    {
        try
        {
            return await _http.GetFromJsonAsync<Result<BusStopDto>>($"/api/bus-stops/{id}");
        }
        catch (TaskCanceledException)
        {
            return Result<BusStopDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<BusStopDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<BusStopDto>?> CreateAsync(CreateBusStopRequest request)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("/api/bus-stops", request);
            return await resp.Content.ReadFromJsonAsync<Result<BusStopDto>>();
        }
        catch (TaskCanceledException)
        {
            return Result<BusStopDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<BusStopDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<BusStopDto>?> UpdateAsync(long id, UpdateBusStopRequest request)
    {
        try
        {
            var resp = await _http.PutAsJsonAsync($"/api/bus-stops/{id}", request);
            return await resp.Content.ReadFromJsonAsync<Result<BusStopDto>>();
        }
        catch (TaskCanceledException)
        {
            return Result<BusStopDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<BusStopDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<bool>?> DeleteAsync(long id)
    {
        try
        {
            var resp = await _http.DeleteAsync($"/api/bus-stops/{id}");
            return await resp.Content.ReadFromJsonAsync<Result<bool>>();
        }
        catch (TaskCanceledException)
        {
            return Result<bool>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<bool>.Failure($"API request failed: {ex.Message}");
        }
    }
}

public class BusRouteService : IBusRouteService
{
    private readonly HttpClient _http;
    public BusRouteService(HttpClient http) => _http = http;

    public async Task<Result<FullRouteResponseDto>> GetFullRouteAsync(long busId)
    {
        try
        {
            return (await _http.GetFromJsonAsync<Result<FullRouteResponseDto>>($"/api/bus-routes/bus/{busId}"))!;
        }
        catch (TaskCanceledException)
        {
            return Result<FullRouteResponseDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<FullRouteResponseDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<bool>> AssignRouteStopsAsync(AssignBusRoutesRequest request)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("/api/bus-routes/assign", request);
            return (await resp.Content.ReadFromJsonAsync<Result<bool>>())!;
        }
        catch (TaskCanceledException)
        {
            return Result<bool>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<bool>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<bool>> ReorderRouteStopsAsync(ReorderBusRoutesRequest request)
    {
        try
        {
            var resp = await _http.PutAsJsonAsync("/api/bus-routes/reorder", request);
            return (await resp.Content.ReadFromJsonAsync<Result<bool>>())!;
        }
        catch (TaskCanceledException)
        {
            return Result<bool>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<bool>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteRouteStopAsync(long busId, int stopOrder)
    {
        try
        {
            var resp = await _http.DeleteAsync($"/api/bus-routes/bus/{busId}/stop/{stopOrder}");
            return (await resp.Content.ReadFromJsonAsync<Result<bool>>())!;
        }
        catch (TaskCanceledException)
        {
            return Result<bool>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<bool>.Failure($"API request failed: {ex.Message}");
        }
    }
}

public class RegionService : IRegionService
{
    private readonly HttpClient _http;
    public RegionService(HttpClient http) => _http = http;

    public async Task<PagedResult<RegionDto>?> GetRegionsAsync(int pageNumber, int pageSize, string? searchName)
    {
        try
        {
            var url = !string.IsNullOrWhiteSpace(searchName)
                ? $"/api/regions/search?pageNumber={pageNumber}&pageSize={pageSize}&regionName={Uri.EscapeDataString(searchName)}"
                : $"/api/regions?pageNumber={pageNumber}&pageSize={pageSize}";
            return await _http.GetFromJsonAsync<PagedResult<RegionDto>>(url);
        }
        catch (TaskCanceledException)
        {
            return PagedResult<RegionDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return PagedResult<RegionDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<RegionDto>?> GetByIdAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<Result<RegionDto>>($"/api/regions/{id}");
        }
        catch (TaskCanceledException)
        {
            return Result<RegionDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<RegionDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<RegionDto>?> CreateAsync(CreateRegionRequest request)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("/api/regions", request);
            return await resp.Content.ReadFromJsonAsync<Result<RegionDto>>();
        }
        catch (TaskCanceledException)
        {
            return Result<RegionDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<RegionDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<RegionDto>?> UpdateAsync(int id, UpdateRegionRequest request)
    {
        try
        {
            var resp = await _http.PutAsJsonAsync($"/api/regions/{id}", request);
            return await resp.Content.ReadFromJsonAsync<Result<RegionDto>>();
        }
        catch (TaskCanceledException)
        {
            return Result<RegionDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<RegionDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<bool>?> DeleteAsync(int id)
    {
        try
        {
            var resp = await _http.DeleteAsync($"/api/regions/{id}");
            return await resp.Content.ReadFromJsonAsync<Result<bool>>();
        }
        catch (TaskCanceledException)
        {
            return Result<bool>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<bool>.Failure($"API request failed: {ex.Message}");
        }
    }
}

public class StoreService : IStoreService
{
    private readonly HttpClient _http;
    public StoreService(HttpClient http) => _http = http;

    public async Task<PagedResult<StoreDto>?> GetStoresAsync(int pageNumber, int pageSize, string? searchName, int? regionId = null)
    {
        try
        {
            string url;
            if (!string.IsNullOrWhiteSpace(searchName))
            {
                url = $"/api/stores/search?pageNumber={pageNumber}&pageSize={pageSize}&searchTerm={Uri.EscapeDataString(searchName)}";
                if (regionId.HasValue) url += $"&regionId={regionId.Value}";
            }
            else
            {
                url = $"/api/stores?pageNumber={pageNumber}&pageSize={pageSize}";
                if (regionId.HasValue) url += $"&regionId={regionId.Value}";
            }
            return await _http.GetFromJsonAsync<PagedResult<StoreDto>>(url);
        }
        catch (TaskCanceledException)
        {
            return PagedResult<StoreDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return PagedResult<StoreDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<StoreDto>?> GetByIdAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<Result<StoreDto>>($"/api/stores/{id}");
        }
        catch (TaskCanceledException)
        {
            return Result<StoreDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<StoreDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<StoreDto>?> CreateAsync(CreateStoreRequest request)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("/api/stores", request);
            return await resp.Content.ReadFromJsonAsync<Result<StoreDto>>();
        }
        catch (TaskCanceledException)
        {
            return Result<StoreDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<StoreDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<StoreDto>?> UpdateAsync(int id, UpdateStoreRequest request)
    {
        try
        {
            var resp = await _http.PutAsJsonAsync($"/api/stores/{id}", request);
            return await resp.Content.ReadFromJsonAsync<Result<StoreDto>>();
        }
        catch (TaskCanceledException)
        {
            return Result<StoreDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<StoreDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<bool>?> DeleteAsync(int id)
    {
        try
        {
            var resp = await _http.DeleteAsync($"/api/stores/{id}");
            return await resp.Content.ReadFromJsonAsync<Result<bool>>();
        }
        catch (TaskCanceledException)
        {
            return Result<bool>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<bool>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<bool>?> AssignNearestStopsAsync(int id, AssignNearestStopsRequest request)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync($"/api/stores/{id}/nearest-stops", request);
            return await resp.Content.ReadFromJsonAsync<Result<bool>>();
        }
        catch (TaskCanceledException)
        {
            return Result<bool>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<bool>.Failure($"API request failed: {ex.Message}");
        }
    }
}

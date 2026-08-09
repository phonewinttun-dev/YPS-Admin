using System.Net.Http.Json;
using YpsAdmin.Shared;

namespace YpsAdmin.Web.Services;

// DTOs matching API specs
public record BusLineDto(int RouteId, int BusNumber, string? OutboundTitleMm, string? OutboundTitleEn, string? ReturnTitleMm, string? ReturnTitleEn, bool IsYpsAccepted);
public record CreateBusLineRequest(int RouteId, int BusNumber, string? OutboundTitleMm, string? OutboundTitleEn, string? ReturnTitleMm, string? ReturnTitleEn, bool IsYpsAccepted);
public record UpdateBusLineRequest(int BusNumber, string? OutboundTitleMm, string? OutboundTitleEn, string? ReturnTitleMm, string? ReturnTitleEn, bool IsYpsAccepted);

public record BusStopDto(int StopId, string NameMm, string? NameEn, int? TownshipId, string? TownshipNameMm, string? TownshipNameEn, string? RoadMm, string? RoadEn, int TotalServingBusLines);
public record CreateBusStopRequest(int? StopId, string NameMm, string? NameEn, int? TownshipId, string? RoadMm, string? RoadEn);
public record UpdateBusStopRequest(string NameMm, string? NameEn, int? TownshipId, string? RoadMm, string? RoadEn);

public record RouteStopDto(int Id, int RouteId, int? StopId, string Direction, int StopOrder, string? StopType, string? StopNameMm, string? StopNameEn, int? TownshipId, string? TownshipNameMm, string? TownshipNameEn, string? RoadMm, string? RoadEn);
public record FullRouteResponseDto(int RouteId, int BusNumber, string? OutboundTitleMm, string? OutboundTitleEn, string? ReturnTitleMm, string? ReturnTitleEn, List<RouteStopDto> OutboundStops, List<RouteStopDto> ReturnStops);
public record AssignRouteStopItem(int? StopId, string Direction, int StopOrder, string? StopType);
public record AssignRouteStopsRequest(int RouteId, List<AssignRouteStopItem> Stops);
public record ReorderItem(int RouteStopId, int NewStopOrder);
public record ReorderRouteStopsRequest(int RouteId, string Direction, List<ReorderItem> Items);

public record NearestStopDto(int Id, string? StopNameMm, string? StopNameEn, int? MatchedStopId);
public record YpsStoreDto(int StoreId, string NameMm, string? NameEn, string? Category, int? TownshipId, string? TownshipNameMm, string? TownshipNameEn, decimal? Latitude, decimal? Longitude, List<NearestStopDto> NearestStops, List<int> ServingBusLines);
public record CreateYpsStoreRequest(int? StoreId, string NameMm, string? NameEn, string? Category, int? TownshipId, decimal? Latitude, decimal? Longitude);
public record UpdateYpsStoreRequest(string NameMm, string? NameEn, string? Category, int? TownshipId, decimal? Latitude, decimal? Longitude);
public record AssignNearestStopItem(int? MatchedStopId, string? StopNameMm, string? StopNameEn);
public record AssignNearestStopsRequest(List<AssignNearestStopItem> NearestStops);
public record AssignServingBusLinesRequest(List<int> BusNumbers);

public record TownshipDto(int TownshipId, string TownshipNameMm, string? TownshipNameEn, bool DeleteFlag);
public record CreateTownshipRequest(string TownshipNameMm, string? TownshipNameEn);
public record UpdateTownshipRequest(string TownshipNameMm, string? TownshipNameEn);

// Services Interfaces
public interface IBusLineService
{
    Task<PagedResult<BusLineDto>?> GetBusLinesAsync(int pageNumber, int pageSize, string? searchBusNumber);
    Task<Result<BusLineDto>?> GetByIdAsync(int id);
    Task<Result<BusLineDto>?> CreateAsync(CreateBusLineRequest request);
    Task<Result<BusLineDto>?> UpdateAsync(int id, UpdateBusLineRequest request);
    Task<Result<bool>?> DeleteAsync(int id);
}

public interface IBusStopService
{
    Task<PagedResult<BusStopDto>?> GetBusStopsAsync(int pageNumber, int pageSize, string? searchStopName, int? townshipId = null);
    Task<Result<BusStopDto>?> GetByIdAsync(int id);
    Task<Result<BusStopDto>?> CreateAsync(CreateBusStopRequest request);
    Task<Result<BusStopDto>?> UpdateAsync(int id, UpdateBusStopRequest request);
    Task<Result<bool>?> DeleteAsync(int id);
}

public interface IRouteStopService
{
    Task<Result<FullRouteResponseDto>> GetFullRouteAsync(int busLineId);
    Task<Result<bool>> AssignRouteStopsAsync(AssignRouteStopsRequest request);
    Task<Result<bool>> ReorderRouteStopsAsync(ReorderRouteStopsRequest request);
    Task<Result<bool>> DeleteRouteStopAsync(int routeStopId);
}

public interface IYpsStoreService
{
    Task<PagedResult<YpsStoreDto>?> GetStoresAsync(int pageNumber, int pageSize, string? searchName, int? townshipId = null);
    Task<Result<YpsStoreDto>?> GetByIdAsync(int id);
    Task<Result<YpsStoreDto>?> CreateAsync(CreateYpsStoreRequest request);
    Task<Result<YpsStoreDto>?> UpdateAsync(int id, UpdateYpsStoreRequest request);
    Task<Result<bool>?> DeleteAsync(int id);
    Task<Result<bool>?> AssignNearestStopsAsync(int id, AssignNearestStopsRequest request);
    Task<Result<bool>?> AssignServingBusLinesAsync(int id, AssignServingBusLinesRequest request);
}

public interface ITownshipService
{
    Task<PagedResult<TownshipDto>?> GetTownshipsAsync(int pageNumber, int pageSize, string? searchName);
    Task<Result<TownshipDto>?> GetByIdAsync(int id);
    Task<Result<TownshipDto>?> CreateAsync(CreateTownshipRequest request);
    Task<Result<TownshipDto>?> UpdateAsync(int id, UpdateTownshipRequest request);
    Task<Result<bool>?> DeleteAsync(int id);
}

// Service Implementations
public class BusLineService : IBusLineService
{
    private readonly HttpClient _http;
    public BusLineService(HttpClient http) => _http = http;

    public async Task<PagedResult<BusLineDto>?> GetBusLinesAsync(int pageNumber, int pageSize, string? searchBusNumber)
    {
        try
        {
            var url = !string.IsNullOrWhiteSpace(searchBusNumber)
                ? $"/api/bus-lines/search?pageNumber={pageNumber}&pageSize={pageSize}&busNumber={Uri.EscapeDataString(searchBusNumber)}"
                : $"/api/bus-lines?pageNumber={pageNumber}&pageSize={pageSize}";
            return await _http.GetFromJsonAsync<PagedResult<BusLineDto>>(url);
        }
        catch (TaskCanceledException)
        {
            return PagedResult<BusLineDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return PagedResult<BusLineDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<BusLineDto>?> GetByIdAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<Result<BusLineDto>>($"/api/bus-lines/{id}");
        }
        catch (TaskCanceledException)
        {
            return Result<BusLineDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<BusLineDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<BusLineDto>?> CreateAsync(CreateBusLineRequest request)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("/api/bus-lines", request);
            return await resp.Content.ReadFromJsonAsync<Result<BusLineDto>>();
        }
        catch (TaskCanceledException)
        {
            return Result<BusLineDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<BusLineDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<BusLineDto>?> UpdateAsync(int id, UpdateBusLineRequest request)
    {
        try
        {
            var resp = await _http.PutAsJsonAsync($"/api/bus-lines/{id}", request);
            return await resp.Content.ReadFromJsonAsync<Result<BusLineDto>>();
        }
        catch (TaskCanceledException)
        {
            return Result<BusLineDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<BusLineDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<bool>?> DeleteAsync(int id)
    {
        try
        {
            var resp = await _http.DeleteAsync($"/api/bus-lines/{id}");
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

    public async Task<PagedResult<BusStopDto>?> GetBusStopsAsync(int pageNumber, int pageSize, string? searchStopName, int? townshipId = null)
    {
        try
        {
            string url;
            if (!string.IsNullOrWhiteSpace(searchStopName))
            {
                url = $"/api/bus-stops/search?pageNumber={pageNumber}&pageSize={pageSize}&searchTerm={Uri.EscapeDataString(searchStopName)}";
                if (townshipId.HasValue) url += $"&townshipId={townshipId.Value}";
            }
            else
            {
                url = $"/api/bus-stops?pageNumber={pageNumber}&pageSize={pageSize}";
                if (townshipId.HasValue) url += $"&townshipId={townshipId.Value}";
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

    public async Task<Result<BusStopDto>?> GetByIdAsync(int id)
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

    public async Task<Result<BusStopDto>?> UpdateAsync(int id, UpdateBusStopRequest request)
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

    public async Task<Result<bool>?> DeleteAsync(int id)
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

public class RouteStopService : IRouteStopService
{
    private readonly HttpClient _http;
    public RouteStopService(HttpClient http) => _http = http;

    public async Task<Result<FullRouteResponseDto>> GetFullRouteAsync(int busLineId)
    {
        try
        {
            return (await _http.GetFromJsonAsync<Result<FullRouteResponseDto>>($"/api/route-stops/bus-line/{busLineId}"))!;
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

    public async Task<Result<bool>> AssignRouteStopsAsync(AssignRouteStopsRequest request)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("/api/route-stops/assign", request);
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

    public async Task<Result<bool>> ReorderRouteStopsAsync(ReorderRouteStopsRequest request)
    {
        try
        {
            var resp = await _http.PutAsJsonAsync("/api/route-stops/reorder", request);
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

    public async Task<Result<bool>> DeleteRouteStopAsync(int routeStopId)
    {
        try
        {
            var resp = await _http.DeleteAsync($"/api/route-stops/{routeStopId}");
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

public class YpsStoreService : IYpsStoreService
{
    private readonly HttpClient _http;
    public YpsStoreService(HttpClient http) => _http = http;

    public async Task<PagedResult<YpsStoreDto>?> GetStoresAsync(int pageNumber, int pageSize, string? searchName, int? townshipId = null)
    {
        try
        {
            string url;
            if (!string.IsNullOrWhiteSpace(searchName))
            {
                url = $"/api/yps-stores/search?pageNumber={pageNumber}&pageSize={pageSize}&townshipName={Uri.EscapeDataString(searchName)}";
            }
            else
            {
                url = $"/api/yps-stores?pageNumber={pageNumber}&pageSize={pageSize}";
                if (townshipId.HasValue) url += $"&townshipId={townshipId.Value}";
            }
            return await _http.GetFromJsonAsync<PagedResult<YpsStoreDto>>(url);
        }
        catch (TaskCanceledException)
        {
            return PagedResult<YpsStoreDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return PagedResult<YpsStoreDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<YpsStoreDto>?> GetByIdAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<Result<YpsStoreDto>>($"/api/yps-stores/{id}");
        }
        catch (TaskCanceledException)
        {
            return Result<YpsStoreDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<YpsStoreDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<YpsStoreDto>?> CreateAsync(CreateYpsStoreRequest request)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("/api/yps-stores", request);
            return await resp.Content.ReadFromJsonAsync<Result<YpsStoreDto>>();
        }
        catch (TaskCanceledException)
        {
            return Result<YpsStoreDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<YpsStoreDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<YpsStoreDto>?> UpdateAsync(int id, UpdateYpsStoreRequest request)
    {
        try
        {
            var resp = await _http.PutAsJsonAsync($"/api/yps-stores/{id}", request);
            return await resp.Content.ReadFromJsonAsync<Result<YpsStoreDto>>();
        }
        catch (TaskCanceledException)
        {
            return Result<YpsStoreDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<YpsStoreDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<bool>?> DeleteAsync(int id)
    {
        try
        {
            var resp = await _http.DeleteAsync($"/api/yps-stores/{id}");
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
            var resp = await _http.PostAsJsonAsync($"/api/yps-stores/{id}/nearest-stops", request);
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

    public async Task<Result<bool>?> AssignServingBusLinesAsync(int id, AssignServingBusLinesRequest request)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync($"/api/yps-stores/{id}/serving-bus-lines", request);
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

public class TownshipService : ITownshipService
{
    private readonly HttpClient _http;
    public TownshipService(HttpClient http) => _http = http;

    public async Task<PagedResult<TownshipDto>?> GetTownshipsAsync(int pageNumber, int pageSize, string? searchName)
    {
        try
        {
            var url = !string.IsNullOrWhiteSpace(searchName)
                ? $"/api/townships/search?pageNumber={pageNumber}&pageSize={pageSize}&townshipName={Uri.EscapeDataString(searchName)}"
                : $"/api/townships?pageNumber={pageNumber}&pageSize={pageSize}";
            return await _http.GetFromJsonAsync<PagedResult<TownshipDto>>(url);
        }
        catch (TaskCanceledException)
        {
            return PagedResult<TownshipDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return PagedResult<TownshipDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<TownshipDto>?> GetByIdAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<Result<TownshipDto>>($"/api/townships/{id}");
        }
        catch (TaskCanceledException)
        {
            return Result<TownshipDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<TownshipDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<TownshipDto>?> CreateAsync(CreateTownshipRequest request)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("/api/townships", request);
            return await resp.Content.ReadFromJsonAsync<Result<TownshipDto>>();
        }
        catch (TaskCanceledException)
        {
            return Result<TownshipDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<TownshipDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<TownshipDto>?> UpdateAsync(int id, UpdateTownshipRequest request)
    {
        try
        {
            var resp = await _http.PutAsJsonAsync($"/api/townships/{id}", request);
            return await resp.Content.ReadFromJsonAsync<Result<TownshipDto>>();
        }
        catch (TaskCanceledException)
        {
            return Result<TownshipDto>.Failure("API request timed out. Please try again.");
        }
        catch (HttpRequestException ex)
        {
            return Result<TownshipDto>.Failure($"API request failed: {ex.Message}");
        }
    }

    public async Task<Result<bool>?> DeleteAsync(int id)
    {
        try
        {
            var resp = await _http.DeleteAsync($"/api/townships/{id}");
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

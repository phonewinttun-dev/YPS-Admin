using YpsAdmin.Shared;

namespace YpsAdmin.Domain.DTOs.Store
{
    public class NearestStopDto
    {
        public int Id { get; set; }
        public string? StopNameMm { get; set; }
        public string? StopNameEn { get; set; }
        public int? MatchedStopId { get; set; }
    }

    public class YpsStoreDto
    {
        public int StoreId { get; set; }
        public string NameMm { get; set; } = null!;
        public string? NameEn { get; set; }
        public string? Category { get; set; }
        public int? TownshipId { get; set; }
        public string? TownshipNameMm { get; set; }
        public string? TownshipNameEn { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public List<NearestStopDto> NearestStops { get; set; } = new();
        public List<string> ServingBusLines { get; set; } = new();
    }

    public class CreateYpsStoreRequest
    {
        public int? StoreId { get; set; }
        public string NameMm { get; set; } = null!;
        public string? NameEn { get; set; }
        public string? Category { get; set; }
        public int? TownshipId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }

    public class UpdateYpsStoreRequest
    {
        public string? NameMm { get; set; }
        public string? NameEn { get; set; }
        public string? Category { get; set; }
        public int? TownshipId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }

    public class AssignNearestStopItem
    {
        public int? MatchedStopId { get; set; }
        public string? StopNameMm { get; set; }
        public string? StopNameEn { get; set; }
    }

    public class AssignNearestStopsRequest
    {
        public List<AssignNearestStopItem> NearestStops { get; set; } = new();
    }

    public class AssignServingBusLinesRequest
    {
        public List<string> BusNumbers { get; set; } = new();
    }

    public class YpsStoreGetRequest : PaginationRequest
    {
        public int? TownshipId { get; set; }
    }

    public class YpsStoreSearchRequest : PaginationRequest
    {
        public string? TownshipName { get; set; }
    }
}

using YpsAdmin.Shared;

namespace YpsAdmin.Domain.DTOs.BusStop
{
    public class BusStopDto
    {
        public int StopId { get; set; }
        public string NameMm { get; set; } = null!;
        public string? NameEn { get; set; }
        public int? TownshipId { get; set; }
        public string? TownshipNameMm { get; set; }
        public string? TownshipNameEn { get; set; }
        public string? RoadMm { get; set; }
        public string? RoadEn { get; set; }
        public int TotalServingBusLines { get; set; }
    }

    public class CreateBusStopRequest
    {
        public int? StopId { get; set; }
        public string NameMm { get; set; } = null!;
        public string? NameEn { get; set; }
        public int? TownshipId { get; set; }
        public string? RoadMm { get; set; }
        public string? RoadEn { get; set; }
    }

    public class UpdateBusStopRequest
    {
        public string NameMm { get; set; } = null!;
        public string? NameEn { get; set; }
        public int? TownshipId { get; set; }
        public string? RoadMm { get; set; }
        public string? RoadEn { get; set; }
    }

    public class BusStopGetRequest : PaginationRequest
    {
        public int? TownshipId { get; set; }
    }

    public class BusStopSearchRequest : PaginationRequest
    {
        public string? SearchTerm { get; set; }
        public int? TownshipId { get; set; }
    }
}

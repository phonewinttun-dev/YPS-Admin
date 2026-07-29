using YpsAdmin.Shared;

namespace YpsAdmin.Domain.DTOs.BusStop
{
    public class BusStopDto
    {
        public string StopId { get; set; } = null!;
        public string NameMm { get; set; } = null!;
        public string? NameEn { get; set; }
        public string? TownshipMm { get; set; }
        public string? TownshipEn { get; set; }
        public string? RoadMm { get; set; }
        public string? RoadEn { get; set; }
        public int TotalServingBusLines { get; set; }
    }

    public class CreateBusStopRequest
    {
        public string StopId { get; set; } = null!;
        public string NameMm { get; set; } = null!;
        public string? NameEn { get; set; }
        public string? TownshipMm { get; set; }
        public string? TownshipEn { get; set; }
        public string? RoadMm { get; set; }
        public string? RoadEn { get; set; }
    }

    public class UpdateBusStopRequest
    {
        public string NameMm { get; set; } = null!;
        public string? NameEn { get; set; }
        public string? TownshipMm { get; set; }
        public string? TownshipEn { get; set; }
        public string? RoadMm { get; set; }
        public string? RoadEn { get; set; }
    }

    public class BusStopQueryFilter : PaginationRequest
    {
        public string? SearchStopName { get; set; }
    }
}

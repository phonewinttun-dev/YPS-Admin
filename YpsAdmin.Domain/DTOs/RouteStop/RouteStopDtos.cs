namespace YpsAdmin.Domain.DTOs.RouteStop
{
    public class RouteStopDto
    {
        public int Id { get; set; }
        public string RouteId { get; set; } = null!;
        public string? StopId { get; set; }
        public string Direction { get; set; } = null!;
        public int StopOrder { get; set; }
        public string? StopType { get; set; }
        public string? StopNameMm { get; set; }
        public string? StopNameEn { get; set; }
        public string? TownshipMm { get; set; }
        public string? TownshipEn { get; set; }
        public string? RoadMm { get; set; }
        public string? RoadEn { get; set; }
    }

    public class AssignRouteStopItem
    {
        public string StopId { get; set; } = null!;
        public string Direction { get; set; } = "Outbound"; // Outbound or Return
        public int StopOrder { get; set; }
        public string? StopType { get; set; }
    }

    public class AssignRouteStopsRequest
    {
        public string RouteId { get; set; } = null!;
        public List<AssignRouteStopItem> Stops { get; set; } = new();
    }

    public class ReorderItem
    {
        public int RouteStopId { get; set; }
        public int NewStopOrder { get; set; }
    }

    public class ReorderRouteStopsRequest
    {
        public string RouteId { get; set; } = null!;
        public string Direction { get; set; } = "Outbound";
        public List<ReorderItem> Items { get; set; } = new();
    }

    public class FullRouteResponseDto
    {
        public string RouteId { get; set; } = null!;
        public string BusNumber { get; set; } = null!;
        public string? OutboundTitleMm { get; set; }
        public string? OutboundTitleEn { get; set; }
        public string? ReturnTitleMm { get; set; }
        public string? ReturnTitleEn { get; set; }
        public List<RouteStopDto> OutboundStops { get; set; } = new();
        public List<RouteStopDto> ReturnStops { get; set; } = new();
    }
}

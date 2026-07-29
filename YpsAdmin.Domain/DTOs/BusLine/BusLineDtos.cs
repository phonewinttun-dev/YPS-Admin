using YpsAdmin.Shared;

namespace YpsAdmin.Domain.DTOs.BusLine
{
    public class BusLineDto
    {
        public string RouteId { get; set; } = null!;
        public string BusNumber { get; set; } = null!;
        public string? OutboundTitleMm { get; set; }
        public string? OutboundTitleEn { get; set; }
        public string? ReturnTitleMm { get; set; }
        public string? ReturnTitleEn { get; set; }
        public bool IsYpsAccepted { get; set; }
    }

    public class CreateBusLineRequest
    {
        public string RouteId { get; set; } = null!;
        public string BusNumber { get; set; } = null!;
        public string? OutboundTitleMm { get; set; }
        public string? OutboundTitleEn { get; set; }
        public string? ReturnTitleMm { get; set; }
        public string? ReturnTitleEn { get; set; }
        public bool IsYpsAccepted { get; set; }
    }

    public class UpdateBusLineRequest
    {
        public string BusNumber { get; set; } = null!;
        public string? OutboundTitleMm { get; set; }
        public string? OutboundTitleEn { get; set; }
        public string? ReturnTitleMm { get; set; }
        public string? ReturnTitleEn { get; set; }
        public bool IsYpsAccepted { get; set; }
    }

    public class BusLineQueryFilter : PaginationRequest
    {
        public string? SearchBusNumber { get; set; }
    }
}

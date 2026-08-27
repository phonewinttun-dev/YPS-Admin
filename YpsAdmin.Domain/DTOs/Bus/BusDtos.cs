using System;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.DTOs.Bus
{
    public class BusDto
    {
        public long Id { get; set; }
        public long BusNumber { get; set; }
        public string? VariantId { get; set; }
        public bool IsCardAccepted { get; set; }
        public bool IsReversed { get; set; }
        public bool DeleteFlag { get; set; }
        public DateOnly CreatedAt { get; set; }
        public DateOnly UpdatedAt { get; set; }
    }

    public class CreateBusRequest
    {
        public long BusNumber { get; set; }
        public string? VariantId { get; set; }
        public bool? IsCardAccepted { get; set; }
        public bool? IsReversed { get; set; }
    }

    public class UpdateBusRequest
    {
        public long BusNumber { get; set; }
        public string? VariantId { get; set; }
        public bool? IsCardAccepted { get; set; }
        public bool? IsReversed { get; set; }
        public bool? DeleteFlag { get; set; }
    }

    public class BusGetRequest : PaginationRequest
    {
    }

    public class BusSearchRequest : PaginationRequest
    {
        public string? BusNumber { get; set; }
        public string? VariantId { get; set; }
    }
}

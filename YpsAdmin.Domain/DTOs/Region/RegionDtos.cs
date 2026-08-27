using System;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.DTOs.Region
{
    public class RegionDto
    {
        public int Id { get; set; }
        public string RegionName { get; set; } = null!;
        public bool DeleteFlag { get; set; }
        public DateOnly CreatedAt { get; set; }
        public DateOnly UpdatedAt { get; set; }
    }

    public class CreateRegionRequest
    {
        public string RegionName { get; set; } = null!;
    }

    public class UpdateRegionRequest
    {
        public string RegionName { get; set; } = null!;
        public bool? DeleteFlag { get; set; }
    }

    public class RegionGetRequest : PaginationRequest
    {
    }

    public class RegionSearchRequest : PaginationRequest
    {
        public string? RegionName { get; set; }
    }
}

using YpsAdmin.Shared;

namespace YpsAdmin.Domain.DTOs.Township
{
    public class TownshipDto
    {
        public int TownshipId { get; set; }
        public string TownshipNameMm { get; set; } = null!;
        public string? TownshipNameEn { get; set; }
        public bool DeleteFlag { get; set; }
    }

    public class CreateTownshipRequest
    {
        public string TownshipNameMm { get; set; } = null!;
        public string? TownshipNameEn { get; set; }
    }

    public class UpdateTownshipRequest
    {
        public string TownshipNameMm { get; set; } = null!;
        public string? TownshipNameEn { get; set; }
    }

    public class TownshipGetRequest : PaginationRequest
    {
    }

    public class TownshipSearchRequest : PaginationRequest
    {
        public string? TownshipName { get; set; }
    }
}

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

    public class TownshipQueryFilter : PaginationRequest
    {
        public string? SearchName { get; set; }
    }
}

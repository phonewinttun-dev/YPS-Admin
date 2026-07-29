using System;
using System.Collections.Generic;

namespace YpsAdmin.Database.AppDbContextModels;

public partial class Tblbusstop
{
    public string StopId { get; set; } = null!;

    public string NameMm { get; set; } = null!;

    public string? NameEn { get; set; }

    public string? TownshipMm { get; set; }

    public string? TownshipEn { get; set; }

    public string? RoadMm { get; set; }

    public string? RoadEn { get; set; }

    public int? TotalServingBusLines { get; set; }

    public virtual ICollection<Tblroutestop> Tblroutestops { get; set; } = new List<Tblroutestop>();

    public virtual ICollection<TblypsstoreNeareststop> TblypsstoreNeareststops { get; set; } = new List<TblypsstoreNeareststop>();
}

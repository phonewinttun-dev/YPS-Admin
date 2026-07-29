using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace YpsAdmin.Database.AppDbContextModels;

public partial class Tblypsstore
{
    public string StoreId { get; set; } = null!;

    public string NameMm { get; set; } = null!;

    public string? NameEn { get; set; }

    public string? Category { get; set; }

    public string? TownshipMm { get; set; }

    public string? TownshipEn { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public Point? Geom { get; set; }

    public virtual ICollection<TblypsstoreNeareststop> TblypsstoreNeareststops { get; set; } = new List<TblypsstoreNeareststop>();

    public virtual ICollection<TblypsstoreServingbusline> TblypsstoreServingbuslines { get; set; } = new List<TblypsstoreServingbusline>();
}

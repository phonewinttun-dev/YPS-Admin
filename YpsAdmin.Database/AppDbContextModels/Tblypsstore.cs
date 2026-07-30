using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace YpsAdmin.Database.AppDbContextModels;

public partial class TblYpsStore
{
    public int StoreId { get; set; }

    public string NameMm { get; set; } = null!;

    public string? NameEn { get; set; }

    public string? Category { get; set; }

    public int? TownshipId { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public Point? Geom { get; set; }

    public virtual ICollection<TblYpsStoreNearestStop> TblYpsStoreNearestStops { get; set; } = new List<TblYpsStoreNearestStop>();

    public virtual ICollection<TblYpsStoreServingBusLine> TblYpsStoreServingBusLines { get; set; } = new List<TblYpsStoreServingBusLine>();

    public virtual TblTownship? Township { get; set; }
}

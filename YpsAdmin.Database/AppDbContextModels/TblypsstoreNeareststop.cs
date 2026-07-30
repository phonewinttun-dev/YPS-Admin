using System;
using System.Collections.Generic;

namespace YpsAdmin.Database.AppDbContextModels;

public partial class TblYpsStoreNearestStop
{
    public int Id { get; set; }

    public int StoreId { get; set; }

    public string? StopNameMm { get; set; }

    public string? StopNameEn { get; set; }

    public int? MatchedStopId { get; set; }

    public virtual TblBusStop? MatchedStop { get; set; }

    public virtual TblYpsStore Store { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace YpsAdmin.Database.AppDbContextModels;

public partial class TblypsstoreNeareststop
{
    public int Id { get; set; }

    public string StoreId { get; set; } = null!;

    public string? StopNameMm { get; set; }

    public string? StopNameEn { get; set; }

    public string? MatchedStopId { get; set; }

    public virtual Tblbusstop? MatchedStop { get; set; }

    public virtual Tblypsstore Store { get; set; } = null!;
}

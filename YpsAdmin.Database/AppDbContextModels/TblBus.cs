using System;
using System.Collections.Generic;

namespace YpsAdmin.Database.AppDbContextModels;

public partial class TblBus
{
    public long Id { get; set; }

    public long BusNumber { get; set; }

    public string? VariantId { get; set; }

    public bool? IsCardAccepted { get; set; }

    public bool? IsReversed { get; set; }

    public bool? DeleteFlag { get; set; }

    public DateOnly CreatedAt { get; set; }

    public DateOnly UpdatedAt { get; set; }

    public virtual ICollection<TblBusRoute> TblBusRoutes { get; set; } = new List<TblBusRoute>();
}

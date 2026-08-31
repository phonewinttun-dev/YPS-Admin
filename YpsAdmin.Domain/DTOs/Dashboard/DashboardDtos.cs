using System;

namespace YpsAdmin.Domain.DTOs.Dashboard
{
    /// <summary>
    /// Represents high-level summary metrics for the YPS Admin dashboard.
    /// </summary>
    /// <param name="TotalBusLines">Total active bus lines registered in the system.</param>
    /// <param name="TotalBusStops">Total active bus stops registered in the network.</param>
    /// <param name="TotalStores">Total active YPS card retail/top-up stores.</param>
    /// <param name="TotalCardAcceptedBuses">Number of active bus lines accepting YPS card payment.</param>
    /// <param name="TotalRegions">Total active operational regions/townships covered.</param>
    /// <param name="TotalRouteMappings">Total active bus route stop sequence mappings.</param>
    public sealed record DashboardSummaryDto(
        long TotalBusLines,
        long TotalBusStops,
        long TotalStores,
        long TotalCardAcceptedBuses,
        long TotalRegions,
        long TotalRouteMappings);
}

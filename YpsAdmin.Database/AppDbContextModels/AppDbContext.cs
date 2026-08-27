using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace YpsAdmin.Database.AppDbContextModels;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblBus> TblBuses { get; set; }

    public virtual DbSet<TblBusStop> TblBusStops { get; set; }

    public virtual DbSet<TblBusRoute> TblBusRoutes { get; set; }

    public virtual DbSet<TblRegion> TblRegions { get; set; }

    public virtual DbSet<TblStore> TblStores { get; set; }

    public virtual DbSet<TblNearestBusStop> TblNearestBusStops { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblBus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("buses_pkey");

            entity.ToTable("buses", "public");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.BusNumber).HasColumnName("bus_number");
            entity.Property(e => e.VariantId).HasColumnName("variant_id");
            entity.Property(e => e.IsCardAccepted).HasColumnName("is_card_accepted");
            entity.Property(e => e.IsReversed).HasColumnName("is_reversed");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<TblBusStop>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bus_stops_pkey");

            entity.ToTable("bus_stops", "public");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.StopName).HasColumnName("stop_name");
            entity.Property(e => e.Lat).HasColumnName("lat");
            entity.Property(e => e.Lon).HasColumnName("lon");
            entity.Property(e => e.RegionId).HasColumnName("region_id");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Region).WithMany(p => p.TblBusStops)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fkpe889tpalwp3ds8tejiudyy6j");
        });

        modelBuilder.Entity<TblBusRoute>(entity =>
        {
            entity.HasKey(e => new { e.BusId, e.StopOrder }).HasName("bus_routes_pkey");

            entity.ToTable("bus_routes", "public");

            entity.Property(e => e.BusId).HasColumnName("bus_id");
            entity.Property(e => e.BusStopId).HasColumnName("bus_stop_id");
            entity.Property(e => e.StopOrder).HasColumnName("stop_order");

            entity.HasOne(d => d.Bus).WithMany(p => p.TblBusRoutes)
                .HasForeignKey(d => d.BusId)
                .HasConstraintName("fkkl7smwo01g1dnb0vlfxehnr9u");

            entity.HasOne(d => d.BusStop).WithMany(p => p.TblBusRoutes)
                .HasForeignKey(d => d.BusStopId)
                .HasConstraintName("fkotrfwa8m0g7x02t44ye6vvhm");
        });

        modelBuilder.Entity<TblRegion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("regions_pkey");

            entity.ToTable("regions", "public");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.RegionName).HasColumnName("region_name");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<TblStore>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("stores_pkey");

            entity.ToTable("stores", "public");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.EngName).HasColumnName("eng_name");
            entity.Property(e => e.MmName).HasColumnName("mm_name");
            entity.Property(e => e.Category).HasColumnName("category");
            entity.Property(e => e.Lat).HasColumnName("lat");
            entity.Property(e => e.Lon).HasColumnName("lon");
            entity.Property(e => e.RegionId).HasColumnName("region_id");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Region).WithMany(p => p.TblStores)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fkcyvqngm141se357optyh4eyfh");
        });

        modelBuilder.Entity<TblNearestBusStop>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("nearest_bus_stops_pkey");

            entity.ToTable("nearest_bus_stops", "public");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.StoreId).HasColumnName("store_id");
            entity.Property(e => e.BusStopId).HasColumnName("bus_stop_id");
            entity.Property(e => e.DistanceKm).HasColumnName("distance_km");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Store).WithMany(p => p.TblNearestBusStops)
                .HasForeignKey(d => d.StoreId)
                .HasConstraintName("fkb8rg6s7mpqex4xscm0qseb9f2");

            entity.HasOne(d => d.BusStop).WithMany(p => p.TblNearestBusStops)
                .HasForeignKey(d => d.BusStopId)
                .HasConstraintName("fk5sx37myi06be6njlm6i63pgk8");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

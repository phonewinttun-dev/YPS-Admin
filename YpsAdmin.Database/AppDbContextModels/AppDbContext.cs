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

    public virtual DbSet<TblBusLine> TblBusLines { get; set; }

    public virtual DbSet<TblBusStop> TblBusStops { get; set; }

    public virtual DbSet<TblRouteStop> TblRouteStops { get; set; }

    public virtual DbSet<TblTownship> TblTownships { get; set; }

    public virtual DbSet<TblYpsStore> TblYpsStores { get; set; }

    public virtual DbSet<TblYpsStoreNearestStop> TblYpsStoreNearestStops { get; set; }

    public virtual DbSet<TblYpsStoreServingBusLine> TblYpsStoreServingBusLines { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<TblBusLine>(entity =>
        {
            entity.HasKey(e => e.RouteId).HasName("TblBusLine_pkey");

            entity.ToTable("TblBusLine");

            entity.Property(e => e.RouteId).ValueGeneratedNever();
            entity.Property(e => e.IsYpsAccepted).HasDefaultValue(false);
            entity.Property(e => e.OutboundTitleEn).HasMaxLength(255);
            entity.Property(e => e.OutboundTitleMm).HasMaxLength(255);
            entity.Property(e => e.ReturnTitleEn).HasMaxLength(255);
            entity.Property(e => e.ReturnTitleMm).HasMaxLength(255);
        });

        modelBuilder.Entity<TblBusStop>(entity =>
        {
            entity.HasKey(e => e.StopId).HasName("TblBusStop_pkey");

            entity.ToTable("TblBusStop");

            entity.Property(e => e.NameEn).HasMaxLength(255);
            entity.Property(e => e.NameMm).HasMaxLength(255);
            entity.Property(e => e.RoadEn).HasMaxLength(255);
            entity.Property(e => e.RoadMm).HasMaxLength(255);
            entity.Property(e => e.TotalServingBusLines).HasDefaultValue(0);

            entity.HasOne(d => d.Township).WithMany(p => p.TblBusStops)
                .HasForeignKey(d => d.TownshipId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("TblBusStop_TownshipId_fkey");
        });

        modelBuilder.Entity<TblRouteStop>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TblRouteStop_pkey");

            entity.ToTable("TblRouteStop");

            entity.HasIndex(e => new { e.RouteId, e.Direction, e.StopOrder }, "UQ_RouteStop_Order").IsUnique();

            entity.Property(e => e.Direction).HasMaxLength(20);
            entity.Property(e => e.StopType).HasMaxLength(50);

            entity.HasOne(d => d.Route).WithMany(p => p.TblRouteStops)
                .HasForeignKey(d => d.RouteId)
                .HasConstraintName("FK_RouteStop_Route");

            entity.HasOne(d => d.Stop).WithMany(p => p.TblRouteStops)
                .HasForeignKey(d => d.StopId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_RouteStop_Stop");
        });

        modelBuilder.Entity<TblTownship>(entity =>
        {
            entity.HasKey(e => e.TownshipId).HasName("TblTownship_pkey");

            entity.ToTable("TblTownship");

            entity.Property(e => e.DeleteFlag).HasDefaultValue(false);
            entity.Property(e => e.TownshipNameEn).HasMaxLength(255);
            entity.Property(e => e.TownshipNameMm).HasMaxLength(255);
        });

        modelBuilder.Entity<TblYpsStore>(entity =>
        {
            entity.HasKey(e => e.StoreId).HasName("TblYpsStore_pkey");

            entity.ToTable("TblYpsStore");

            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.Geom).HasColumnType("geometry(Point,4326)");
            entity.Property(e => e.Latitude).HasPrecision(10, 7);
            entity.Property(e => e.Longitude).HasPrecision(10, 7);
            entity.Property(e => e.NameEn).HasMaxLength(255);
            entity.Property(e => e.NameMm).HasMaxLength(255);

            entity.HasOne(d => d.Township).WithMany(p => p.TblYpsStores)
                .HasForeignKey(d => d.TownshipId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("TblYpsStore_TownshipId_fkey");
        });

        modelBuilder.Entity<TblYpsStoreNearestStop>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TblYpsStore_NearestStop_pkey");

            entity.ToTable("TblYpsStore_NearestStop");

            entity.HasIndex(e => new { e.StoreId, e.MatchedStopId }, "UQ_YpsStore_Nearest").IsUnique();

            entity.Property(e => e.StopNameEn).HasMaxLength(255);
            entity.Property(e => e.StopNameMm).HasMaxLength(255);

            entity.HasOne(d => d.MatchedStop).WithMany(p => p.TblYpsStoreNearestStops)
                .HasForeignKey(d => d.MatchedStopId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_YpsStore_Nearest_Stop");

            entity.HasOne(d => d.Store).WithMany(p => p.TblYpsStoreNearestStops)
                .HasForeignKey(d => d.StoreId)
                .HasConstraintName("FK_YpsStore_Nearest_Store");
        });

        modelBuilder.Entity<TblYpsStoreServingBusLine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TblYpsStore_ServingBusLine_pkey");

            entity.ToTable("TblYpsStore_ServingBusLine");

            entity.HasIndex(e => new { e.StoreId, e.RouteId }, "UQ_YpsStore_Serving").IsUnique();

            entity.HasOne(d => d.Route).WithMany(p => p.TblYpsStoreServingBusLines)
                .HasForeignKey(d => d.RouteId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_YpsStore_Serving_Route");

            entity.HasOne(d => d.Store).WithMany(p => p.TblYpsStoreServingBusLines)
                .HasForeignKey(d => d.StoreId)
                .HasConstraintName("FK_YpsStore_Serving_Store");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

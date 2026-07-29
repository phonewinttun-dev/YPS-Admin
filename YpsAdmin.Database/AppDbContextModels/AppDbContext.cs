using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace YpsAdmin.Database.AppDbContextModels;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Tblbusline> Tblbuslines { get; set; }

    public virtual DbSet<Tblbusstop> Tblbusstops { get; set; }

    public virtual DbSet<Tblroutestop> Tblroutestops { get; set; }

    public virtual DbSet<Tblypsstore> Tblypsstores { get; set; }

    public virtual DbSet<TblypsstoreNeareststop> TblypsstoreNeareststops { get; set; }

    public virtual DbSet<TblypsstoreServingbusline> TblypsstoreServingbuslines { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<Tblbusline>(entity =>
        {
            entity.HasKey(e => e.RouteId).HasName("tblbusline_pkey");

            entity.ToTable("tblbusline");

            entity.Property(e => e.RouteId)
                .HasMaxLength(50)
                .HasColumnName("route_id");
            entity.Property(e => e.BusNumber)
                .HasMaxLength(50)
                .HasColumnName("bus_number");
            entity.Property(e => e.IsYpsAccepted)
                .HasDefaultValue(false)
                .HasColumnName("is_yps_accepted");
            entity.Property(e => e.OutboundTitleEn)
                .HasMaxLength(255)
                .HasColumnName("outbound_title_en");
            entity.Property(e => e.OutboundTitleMm)
                .HasMaxLength(255)
                .HasColumnName("outbound_title_mm");
            entity.Property(e => e.ReturnTitleEn)
                .HasMaxLength(255)
                .HasColumnName("return_title_en");
            entity.Property(e => e.ReturnTitleMm)
                .HasMaxLength(255)
                .HasColumnName("return_title_mm");
        });

        modelBuilder.Entity<Tblbusstop>(entity =>
        {
            entity.HasKey(e => e.StopId).HasName("tblbusstop_pkey");

            entity.ToTable("tblbusstop");

            entity.Property(e => e.StopId)
                .HasMaxLength(50)
                .HasColumnName("stop_id");
            entity.Property(e => e.NameEn)
                .HasMaxLength(255)
                .HasColumnName("name_en");
            entity.Property(e => e.NameMm)
                .HasMaxLength(255)
                .HasColumnName("name_mm");
            entity.Property(e => e.RoadEn)
                .HasMaxLength(255)
                .HasColumnName("road_en");
            entity.Property(e => e.RoadMm)
                .HasMaxLength(255)
                .HasColumnName("road_mm");
            entity.Property(e => e.TotalServingBusLines)
                .HasDefaultValue(0)
                .HasColumnName("total_serving_bus_lines");
            entity.Property(e => e.TownshipEn)
                .HasMaxLength(255)
                .HasColumnName("township_en");
            entity.Property(e => e.TownshipMm)
                .HasMaxLength(255)
                .HasColumnName("township_mm");
        });

        modelBuilder.Entity<Tblroutestop>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tblroutestop_pkey");

            entity.ToTable("tblroutestop");

            entity.HasIndex(e => e.RouteId, "idx_route_stops_route_id");

            entity.HasIndex(e => e.StopId, "idx_route_stops_stop_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Direction)
                .HasMaxLength(20)
                .HasColumnName("direction");
            entity.Property(e => e.RouteId)
                .HasMaxLength(50)
                .HasColumnName("route_id");
            entity.Property(e => e.StopId)
                .HasMaxLength(50)
                .HasColumnName("stop_id");
            entity.Property(e => e.StopOrder).HasColumnName("stop_order");
            entity.Property(e => e.StopType)
                .HasMaxLength(50)
                .HasColumnName("stop_type");

            entity.HasOne(d => d.Route).WithMany(p => p.Tblroutestops)
                .HasForeignKey(d => d.RouteId)
                .HasConstraintName("fk_route");

            entity.HasOne(d => d.Stop).WithMany(p => p.Tblroutestops)
                .HasForeignKey(d => d.StopId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_stop");
        });

        modelBuilder.Entity<Tblypsstore>(entity =>
        {
            entity.HasKey(e => e.StoreId).HasName("tblypsstore_pkey");

            entity.ToTable("tblypsstore");

            entity.HasIndex(e => e.Geom, "idx_yps_stores_geom").HasMethod("gist");

            entity.Property(e => e.StoreId)
                .HasMaxLength(50)
                .HasColumnName("store_id");
            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .HasColumnName("category");
            entity.Property(e => e.Geom)
                .HasColumnType("geometry(Point,4326)")
                .HasColumnName("geom");
            entity.Property(e => e.Latitude)
                .HasPrecision(10, 7)
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasPrecision(10, 7)
                .HasColumnName("longitude");
            entity.Property(e => e.NameEn)
                .HasMaxLength(255)
                .HasColumnName("name_en");
            entity.Property(e => e.NameMm)
                .HasMaxLength(255)
                .HasColumnName("name_mm");
            entity.Property(e => e.TownshipEn)
                .HasMaxLength(255)
                .HasColumnName("township_en");
            entity.Property(e => e.TownshipMm)
                .HasMaxLength(255)
                .HasColumnName("township_mm");
        });

        modelBuilder.Entity<TblypsstoreNeareststop>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tblypsstore_neareststop_pkey");

            entity.ToTable("tblypsstore_neareststop");

            entity.HasIndex(e => e.StoreId, "idx_yps_nearest_store_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MatchedStopId)
                .HasMaxLength(50)
                .HasColumnName("matched_stop_id");
            entity.Property(e => e.StopNameEn)
                .HasMaxLength(255)
                .HasColumnName("stop_name_en");
            entity.Property(e => e.StopNameMm)
                .HasMaxLength(255)
                .HasColumnName("stop_name_mm");
            entity.Property(e => e.StoreId)
                .HasMaxLength(50)
                .HasColumnName("store_id");

            entity.HasOne(d => d.MatchedStop).WithMany(p => p.TblypsstoreNeareststops)
                .HasForeignKey(d => d.MatchedStopId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_matched_stop");

            entity.HasOne(d => d.Store).WithMany(p => p.TblypsstoreNeareststops)
                .HasForeignKey(d => d.StoreId)
                .HasConstraintName("fk_store_nearest");
        });

        modelBuilder.Entity<TblypsstoreServingbusline>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tblypsstore_servingbusline_pkey");

            entity.ToTable("tblypsstore_servingbusline");

            entity.HasIndex(e => e.StoreId, "idx_yps_serving_store_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BusNumber)
                .HasMaxLength(50)
                .HasColumnName("bus_number");
            entity.Property(e => e.StoreId)
                .HasMaxLength(50)
                .HasColumnName("store_id");

            entity.HasOne(d => d.Store).WithMany(p => p.TblypsstoreServingbuslines)
                .HasForeignKey(d => d.StoreId)
                .HasConstraintName("fk_store_serving");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

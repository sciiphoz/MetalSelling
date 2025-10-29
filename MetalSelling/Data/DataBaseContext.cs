using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MetalSelling.Data;

public partial class DataBaseContext : DbContext
{
    public DataBaseContext()
    {
    }

    public DataBaseContext(DbContextOptions<DataBaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<ClientType> ClientTypes { get; set; }

    public virtual DbSet<History> Histories { get; set; }

    public virtual DbSet<MetalType> MetalTypes { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Supply> Supplies { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=MetalSelling;Username=postgres;Password=123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Client_pkey");

            entity.ToTable("Client");

            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PersonalDiscount).HasPrecision(4, 2);
            entity.Property(e => e.Phone).HasMaxLength(20);

            entity.HasOne(d => d.IdClientTypeNavigation).WithMany(p => p.Clients)
                .HasForeignKey(d => d.IdClientType)
                .HasConstraintName("Client_IdClientType_fkey");
        });

        modelBuilder.Entity<ClientType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ClientType_pkey");

            entity.ToTable("ClientType");

            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<History>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("History_pkey");

            entity.ToTable("History");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.Histories)
                .HasForeignKey(d => d.IdProduct)
                .HasConstraintName("History_IdProduct_fkey");
        });

        modelBuilder.Entity<MetalType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("MetalType_pkey");

            entity.ToTable("MetalType");

            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Order_pkey");

            entity.ToTable("Order");

            entity.Property(e => e.Price).HasPrecision(11, 2);
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdClient)
                .HasConstraintName("Order_IdClient_fkey");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdProduct)
                .HasConstraintName("Order_IdProduct_fkey");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Product_pkey");

            entity.ToTable("Product");

            entity.Property(e => e.PricePerPiece).HasPrecision(7, 2);
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.IdMetalTypeNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdMetalType)
                .HasConstraintName("Product_IdMetalType_fkey");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Supplier_pkey");

            entity.ToTable("Supplier");

            entity.Property(e => e.PricePerPiece).HasPrecision(7, 2);
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<Supply>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Supply_pkey");

            entity.ToTable("Supply");

            entity.Property(e => e.Price).HasPrecision(11, 2);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.Supplies)
                .HasForeignKey(d => d.IdProduct)
                .HasConstraintName("Supply_IdProduct_fkey");

            entity.HasOne(d => d.IdSupplierNavigation).WithMany(p => p.Supplies)
                .HasForeignKey(d => d.IdSupplier)
                .HasConstraintName("Supply_IdSupplier_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

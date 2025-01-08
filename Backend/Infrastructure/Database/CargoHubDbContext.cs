using Backend.Features;
using Backend.Features.Clients;
using Backend.Features.Contacts;
using Backend.Features.Inventories;
using Backend.Features.ItemGroups;
using Backend.Features.ItemLines;
using Backend.Features.Items;
using Backend.Features.ItemTypes;
using Backend.Features.Locations;
using Backend.Features.Orders;
using Backend.Features.Shipments;
using Backend.Features.Suppliers;
using Backend.Features.Transfers;
using Backend.Features.Warehouses;
using Backend.Features.Logs;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Database;

public class CargoHubDbContext(DbContextOptions<CargoHubDbContext> options) : DbContext(options)
{
    public virtual DbSet<Client>? Clients { get; set; }
    public virtual DbSet<Contact>? Contacts { get; set; }
    public virtual DbSet<Inventory>? Inventories { get; set; }
    public virtual DbSet<ItemGroup>? ItemGroups { get; set; }
    public virtual DbSet<ItemLine>? ItemLines { get; set; }
    public virtual DbSet<ItemType>? ItemTypes { get; set; }
    public virtual DbSet<Item>? Items { get; set; }
    public virtual DbSet<Location>? Locations { get; set; }
    public virtual DbSet<Order>? Orders { get; set; }
    public virtual DbSet<Shipment>? Shipments { get; set; }
    public virtual DbSet<Supplier>? Suppliers { get; set; }
    public virtual DbSet<Transfer>? Transfers { get; set; }
    public virtual DbSet<Warehouse>? Warehouses { get; set; }

    public virtual DbSet<Log>? Logs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType).Property(nameof(BaseEntity.CreatedAt))
                    .HasDefaultValueSql("GETUTCDATE()");
            }
        }
    }

    public override int SaveChanges()
    {
        ApplyTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry)
            {
                case { State: EntityState.Added }:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case { State: EntityState.Modified }:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
    }
}

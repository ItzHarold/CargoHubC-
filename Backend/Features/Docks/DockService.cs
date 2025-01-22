using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Infrastructure.Database;

namespace Backend.Features.Docks
{
    public interface IDockService
    {
        void CreateDock(Dock dock);
        bool UpdateDock(int id, Dock dock);
        bool ClearDock(int id);
        Dock? GetDockById(int id);
        bool DeleteDock(int id);
        IEnumerable<Dock> GetAllDocks();
    }

    public class DockService : IDockService
    {
        private readonly CargoHubDbContext _dbContext;

        public DockService(CargoHubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void CreateDock(Dock dock)
        {
            if (_dbContext.Docks == null)
            {
                throw new InvalidOperationException("Docks DbSet is null.");
            }

            if (dock.WarehouseId <= 0)
            {
                throw new ArgumentException("WarehouseId is required when creating a dock.");
            }

            // Auto-generate a unique name if not provided
            if (string.IsNullOrWhiteSpace(dock.Name))
            {
                var existingDocks = _dbContext.Docks?.Count() ?? 0;
                dock.Name = $"Dock {(char)('A' + existingDocks)}"; // Generates Dock A, Dock B, etc.
            }

            dock.ShipmentId = 0; // Default to 0 (unoccupied)
            dock.Status = "unoccupied"; // Default status
            dock.CreatedAt = DateTime.Now;

            _dbContext.Docks?.Add(dock);
            _dbContext.SaveChanges();
        }

        public bool UpdateDock(int id, Dock dock)
        {
            var existingDock = _dbContext.Docks?.FirstOrDefault(d => d.Id == id);
            if (existingDock == null) return false;

            existingDock.Name = dock.Name;

            // Prevent manually updating Status, it is automatically handled by ShipmentId
            // Ensure the status is always derived from ShipmentId
            existingDock.ShipmentId = dock.ShipmentId;
            existingDock.Status = existingDock.ShipmentId > 0 ? "occupied" : "unoccupied"; // Automatically set Status based on ShipmentId

            existingDock.UpdatedAt = DateTime.Now;

            // Prevent WarehouseId from being updated
            _dbContext.Docks?.Update(existingDock);
            _dbContext.SaveChanges();
            return true;
        }

        public bool ClearDock(int id)
        {
            var dock = _dbContext.Docks?.FirstOrDefault(d => d.Id == id);
            if (dock == null) return false;

            dock.Status = "unoccupied"; // Clear status
            dock.ShipmentId = 0; // Reset ShipmentId to 0
            dock.UpdatedAt = DateTime.Now;

            _dbContext.Docks?.Update(dock);
            _dbContext.SaveChanges();
            return true;
        }

        public Dock? GetDockById(int id)
        {
            return _dbContext.Docks?.FirstOrDefault(d => d.Id == id);
        }

        public bool DeleteDock(int id)
        {
            var dock = _dbContext.Docks?.FirstOrDefault(d => d.Id == id);
            if (dock == null) return false;

            _dbContext.Docks?.Remove(dock);
            _dbContext.SaveChanges();
            return true;
        }

        public IEnumerable<Dock> GetAllDocks()
        {
            return _dbContext.Docks?.ToList() ?? new List<Dock>();
        }
    }
}

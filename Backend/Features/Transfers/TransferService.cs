using System.Collections.Generic;
using System.Linq;
using Backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Transfers
{
    public interface ITransferService
    {
        IEnumerable<Transfer> GetAllTransfers();
        Transfer? GetTransferById(int id);
        void AddTransfer(Transfer transfer);
        void UpdateTransfer(Transfer transfer);
        void DeleteTransfer(int id);
    }

    public class TransferService : ITransferService
    {
        private readonly CargoHubDbContext _dbContext;

        public TransferService(CargoHubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Transfer> GetAllTransfers()
        {
            return _dbContext.Transfers?.ToList() ?? new List<Transfer>();
        }

        public void AddTransfer(Transfer transfer)
        {
            transfer.CreatedAt = DateTime.Now;

            // Add transfer to DB
            _dbContext.Transfers?.Add(transfer);
            _dbContext.SaveChanges();
        }

        public Transfer? GetTransferById(int id)
        {
            // Use eager loading to load items along with the transfer
            return _dbContext.Transfers?
                .Include(t => t.Items)  // Eager load Items
                .FirstOrDefault(t => t.Id == id);
        }

        public void UpdateTransfer(Transfer transfer)
        {
            transfer.UpdatedAt = DateTime.Now;
            _dbContext.Transfers?.Update(transfer);
            _dbContext.SaveChanges();
        }

        public void DeleteTransfer(int id)
        {
            var transfer = _dbContext.Transfers?.Find(id);
            if (transfer != null)
            {
                _dbContext.Transfers?.Remove(transfer);
                _dbContext.SaveChanges();
            }
        }
    }
}

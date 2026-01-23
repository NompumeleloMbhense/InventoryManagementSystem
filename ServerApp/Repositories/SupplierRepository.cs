using Microsoft.EntityFrameworkCore;
using ServerApp.Data;
using SharedApp.Models;

/// <summary>
/// Repository implementation for managing suppliers.
/// Provides methods for CRUD operations and searching suppliers.
/// </summary>

namespace ServerApp.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly AppDbContext _db;

        public SupplierRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Supplier>> GetPaginatedAsync(int pageNumber, int pageSize)
        {
            return await _db.Suppliers
            .Include(s => s.Products)
            .OrderBy(s => s.SupplierId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        }


        // Total suppliers for pagination
        public async Task<int> GetTotalCountAsync()
        {
           return await _db.Suppliers.CountAsync();
        }


        public async Task<Supplier?> GetByIdAsync(int id)
        {
            return await _db.Suppliers.Include(s => s.Products)
                            .FirstOrDefaultAsync(s => s.SupplierId == id);
        }

        public async Task AddAsync(Supplier supplier)
        {
            _db.Suppliers.Add(supplier);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Supplier supplier)
        {
            _db.Suppliers.Update(supplier);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Find the supplier including their products
            var supplier = await _db.Suppliers.Include(s => s.Products)
                                             .FirstOrDefaultAsync(s => s.SupplierId == id);

            if (supplier is null)
                return false;

            // Check if the supplier has any products
            if (supplier.Products.Any())
                throw new InvalidOperationException("Cannot delete supplier with products. Reassign or delete products first.");

            // Safe to delete
            _db.Suppliers.Remove(supplier);
            await _db.SaveChangesAsync();

            return true;
        }

        // Check if a supplier exists by ID
        public async Task<bool> SupplierExistsAsync(int supplierId)
        {
            return await _db.Suppliers.AnyAsync(s => s.SupplierId == supplierId);
        }
    }
}
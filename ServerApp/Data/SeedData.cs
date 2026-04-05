using Microsoft.EntityFrameworkCore;
using SharedApp.Models;

/// <summary>
/// Seeds initial data into the database 
/// Suppliers and Products.
/// </summary>

namespace ServerApp.Data
{
    public static class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Auto-migrate on startup
            if (db.Database.GetPendingMigrations().Any())
                db.Database.Migrate();

            if (!db.Suppliers.Any())
            {
                var techWorld = new Supplier { Name = "Tech World", Location = "Roodepoort", Email = "contact@techworld.co.za" };
                var soundCo = new Supplier { Name = "Sound Co", Location = "Randburg", Email = "info@soundco.co.za" };
                var homeEssentials = new Supplier { Name = "Home Essentials", Location = "Soweto", Email = "sales@homeessentials.co.za" };
                var gadgetHub = new Supplier { Name = "Gadget Hub", Location = "Sandton", Email = "hello@gadgethub.co.za" };
                var officeSupplies = new Supplier { Name = "Office Supplies", Location = "Johannesburg CBD", Email = "support@officesupplies.co.za" };

                db.Suppliers.AddRange(techWorld, soundCo, homeEssentials, gadgetHub, officeSupplies);
                db.SaveChanges(); // This generates the Supplier IDs

                if (!db.Products.Any())
                {
                    db.Products.AddRange(
                        new Product { Name = "Laptop", Price = 12000.50M, Stock = 25, Category = "Electronics", SupplierId = techWorld.SupplierId },
                        new Product { Name = "Smartphone", Price = 8000.00M, Stock = 50, Category = "Electronics", SupplierId = techWorld.SupplierId },
                        new Product { Name = "Headphones", Price = 450.00M, Stock = 100, Category = "Audio", SupplierId = soundCo.SupplierId },
                        new Product { Name = "Bluetooth Speaker", Price = 1200.00M, Stock = 40, Category = "Audio", SupplierId = soundCo.SupplierId },
                        new Product { Name = "Vacuum Cleaner", Price = 2500.00M, Stock = 15, Category = "Home Appliances", SupplierId = homeEssentials.SupplierId },
                        new Product { Name = "Coffee Maker", Price = 1800.00M, Stock = 30, Category = "Home Appliances", SupplierId = homeEssentials.SupplierId },
                        new Product { Name = "Tablet", Price = 5000.00M, Stock = 20, Category = "Electronics", SupplierId = gadgetHub.SupplierId },
                        new Product { Name = "Wireless Mouse", Price = 300.00M, Stock = 150, Category = "Accessories", SupplierId = gadgetHub.SupplierId },
                        new Product { Name = "Printer", Price = 4000.00M, Stock = 10, Category = "Office Equipment", SupplierId = officeSupplies.SupplierId },
                        new Product { Name = "Desk Chair", Price = 1500.00M, Stock = 25, Category = "Furniture", SupplierId = officeSupplies.SupplierId }
                    );
                    db.SaveChanges();
                }
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SharedApp.Models;

namespace ServerApp.Data
{
    public static class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Apply pending migrations
            if (db.Database.GetPendingMigrations().Any())
            {
                db.Database.Migrate();
            }


            // Seed suppliers if none exist
            if (!db.Suppliers.Any())
            {

                // Add suppliers first
                var suppliers = new[]
                {
                    new Supplier { Name = "Tech World", Location = "Roodepoort",  Email = "contact@techworld.co.za"},
                    new Supplier { Name = "Sound Co", Location = "Randburg", Email = "info@soundco.co.za"},
                    new Supplier { Name = "Home Essentials", Location = "Soweto", Email = "sales@homeessentials.co.za" },
                    new Supplier { Name = "Gadget Hub", Location = "Sandton", Email = "hello@gadgethub.co.za" },
                    new Supplier { Name = "Office Supplies", Location = "Johannesburg CBD", Email = "support@officesupplies.co.za" }
                };


                db.Suppliers.AddRange(suppliers);
                db.SaveChanges(); // Save suppliers to get IDs
            }

            // Seed products if none exist
            if (!db.Products.Any())
            {
                var techWorld = db.Suppliers.First(s => s.Name == "Tech World");
                var soundCo = db.Suppliers.First(s => s.Name == "Sound Co");
                var homeEssentials = db.Suppliers.First(s => s.Name == "Home Essentials");
                var gadgetHub = db.Suppliers.First(s => s.Name == "Gadget Hub");
                var officeSupplies = db.Suppliers.First(s => s.Name == "Office Supplies");

                var products = new[]
               {
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
                };

                db.Products.AddRange(products);
                db.SaveChanges();

                
            }
        }
    }
}

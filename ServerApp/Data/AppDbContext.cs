using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedApp.Models;
using ServerApp.Models;

namespace ServerApp.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationship between Product and Supplier
            modelBuilder.Entity<Product>()
                        .HasOne(p => p.Supplier) // each Product has one Supplier
                        .WithMany(s => s.Products) // each Supplier has many Products
                        .HasForeignKey(p => p.SupplierId) // foreign key in Product table
                        .OnDelete(DeleteBehavior.Cascade); // delete products when supplier is deleted

            // specify precision for Price property
            modelBuilder.Entity<Product>()
                        .Property(p => p.Price)
                        .HasPrecision(18, 2);
        }
    }
}
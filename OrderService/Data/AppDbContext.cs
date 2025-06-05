using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Data
{
    public class AppDbContext : DbContext // Inherit from DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; } = null!; // Use null-forgiving operator
        public DbSet<OrderItem> OrderItems { get; set; } = null!; // Use null-forgiving operator
        public object Products { get; internal set; }
    }
}
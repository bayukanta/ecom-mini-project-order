using Microsoft.EntityFrameworkCore;
using DAL.Models;

namespace DAL
{
    public class MiniProjectOrderDbContext : DbContext
    {
        public MiniProjectOrderDbContext(DbContextOptions<MiniProjectOrderDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //add index here
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> ProductDetails { get; set; }
    }
}

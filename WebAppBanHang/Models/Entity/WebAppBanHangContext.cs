using Microsoft.EntityFrameworkCore;
using WebAppBanHang.Models.Entity;
namespace WebAppBanHang.Models.Entity
{
    public class WebAppBanHangContext: DbContext
    {
        public WebAppBanHangContext()
        {
        }

        public WebAppBanHangContext(DbContextOptions<WebAppBanHangContext> options)
            : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Discount> Discounts { get; set; }


        //xoa user thi xoa order và orderdetail, xoa order thi xoa orderdetail
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // xoa user thi xoa order và orderdetail
            modelBuilder.Entity<Order>()
                .HasOne<User>()
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            // xoa order thi xoa orderdetail
            modelBuilder.Entity<OrderDetail>()
                .HasOne<Order>()
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);            
        }
    }
}

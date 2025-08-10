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
    }
}

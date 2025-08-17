using WebAppBanHang.Models.Entity;
namespace WebAppBanHang.ViewModels
{
    public class AllTablesViewModel
    {
        public List<Product> Products { get; set; }
        public List<User> Users { get; set; }
        public List<Order> Orders { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public List<Discount> Discounts { get; set; }
        public List<ProductDiscount> ProductDiscounts { get; set; }

    }
}

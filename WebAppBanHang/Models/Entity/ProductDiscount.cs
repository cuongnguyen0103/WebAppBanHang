using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace WebAppBanHang.Models.Entity
{
    public class ProductDiscount
    {
        [Key]
        public int ProductDiscountId { get; set; }
        public int ProductId { get; set; }
        public int DiscountId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Quan he voi Product
        [ForeignKey("ProductId")]
        [InverseProperty("ProductDiscounts")]
        public virtual Product Product { get; set; }
        // Quan he voi Discount
        [ForeignKey("DiscountId")]
        [InverseProperty("ProductDiscounts")]
        public virtual Discount Discount { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppBanHang.Models.Entity
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập tên sản phẩm")]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập giá sản phẩm")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số lượng hàng tồn kho")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho không được âm")]
        public int StockQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        // Quan he voi OrderDetail
        [InverseProperty("Product")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        // Quan he voi ProductDiscount
        [InverseProperty("Product")]
        public virtual ICollection<ProductDiscount> ProductDiscounts { get; set; } = new List<ProductDiscount>();
    }
}

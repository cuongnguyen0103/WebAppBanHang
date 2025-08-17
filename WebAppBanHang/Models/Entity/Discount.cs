using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppBanHang.Models.Entity
{
    public class Discount
    {
        [Key]
        public int DiscountId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã giảm giá")]
        public string? Code { get; set; }
        public string? DiscountDescription { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập giá trị giảm")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal DiscountPercent { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Ngày bắt đầu")]

        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Ngày kết thúc")]

        public DateTime EndDate { get; set; }       
        
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Quan he voi OrderDetail
        [InverseProperty("Discount")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        // Quan he voi ProductDiscount
        [InverseProperty("Discount")]
        public virtual ICollection<ProductDiscount> ProductDiscounts { get; set; } = new List<ProductDiscount>();
    }
}

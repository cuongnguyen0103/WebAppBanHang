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

        [Required(ErrorMessage = "Vui lòng nhập phần trăm giảm")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Ngày bắt đầu")]

        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Ngày kết thúc")]

        public DateTime EndDate { get; set; }
        
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Quan he voi OrderDetail
        [InverseProperty("Discount")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}

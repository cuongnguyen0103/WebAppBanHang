using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppBanHang.Models.Entity
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Quantity { get; set; }

        public int? DiscountId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;        

        // Quan he voi Order
        [ForeignKey("OrderId")]
        [InverseProperty("OrderDetails")]
        public virtual Order Order { get; set; }

        // Quan he voi Product
        [ForeignKey("ProductId")]
        [InverseProperty("OrderDetails")]
        public virtual Product Product { get; set; }

        // Quan he voi Discount
        [ForeignKey("DiscountId")]
        [InverseProperty("OrderDetails")]
        public virtual Discount? Discount { get; set; }

    }
}

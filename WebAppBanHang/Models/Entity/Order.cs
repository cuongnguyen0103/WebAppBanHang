using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppBanHang.Models.Entity
{
    public class Order
    {

        [Key]
        public int OrderId { get; set; }

        public int UserId { get; set; }

        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = "Pending"; // Trạng thái đơn hàng, có thể là "Pending", "Processing", "Completed", "Cancelled", v.v.

        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsActive { get; set; } = true;

        //Quan he voi User
        [ForeignKey("UserId")]
        [InverseProperty("Orders")]
        public virtual User User { get; set; }

        //Quan he voi OrderDetail
        [InverseProperty("Order")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppBanHang.Models.Entity
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "UserName không được để trống")]
        [StringLength(100, ErrorMessage = "UserName không được vượt quá 50 ký tự")]
        [MinLength(3, ErrorMessage = "UserName phải có ít nhất 3 ký tự")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; }

        [NotMapped] // Not mapped to the database, used for model validation
        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string? ConfirmPassword { get; set; }

        public string? Role { get; set; } = "customer";


        [Column(TypeName = "nvarchar(50)")]
        [StringLength(50, ErrorMessage = "FullName không được vượt quá 50 ký tự")]
        public string? FullName { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        //Quan he voi Order
        [InverseProperty("User")]
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
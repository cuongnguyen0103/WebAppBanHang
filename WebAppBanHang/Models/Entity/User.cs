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
        [MinLength(5, ErrorMessage = "UserName phải có ít nhất 3 ký tự")]
        public string UserName { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "FullName không được để trống")]
        [StringLength(100, ErrorMessage = "FullName không được vượt quá 50 ký tự")]
        [MinLength(5, ErrorMessage = "FullName phải có ít nhất 3 ký tự")]
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }
        [Required(ErrorMessage = "PhoneNumber không được để trống")]
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Shopping.Models.ViewModels
{
    public class NewPassViewModel
    {
        [Required]
        public string Email { get; set; }
        [DataType(DataType.Password),Required(ErrorMessage = "Vui lòng nhập mật khẩu mới!")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới!")]
        [DataType(DataType.Password),Compare("Password",ErrorMessage = "Mật khẩu xác nhận không trùng khớp!")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Token { get; set; }
    }
}

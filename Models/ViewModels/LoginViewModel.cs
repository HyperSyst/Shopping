using System.ComponentModel.DataAnnotations;

namespace Shopping.Models.ViewModels
{
    public class LoginViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Hãy nhập username!")]
        public string Username { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage = "Hãy nhập mật khẩu!")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }

    }
}

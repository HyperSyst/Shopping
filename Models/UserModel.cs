using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Hãy nhập username!")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Hãy nhập email!")]
        public string Email { get; set; }
        [DataType(DataType.Password),Required(ErrorMessage = "Hãy nhập mật khẩu!")]
        public string Password { get; set; }
    }
}

using Shopping.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping.Models
{
    public class ContactModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề website!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập thông tin liên hệ!")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ!")]
        public string Map { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại!")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập email liên hệ!")]
        public string Email { get; set; }
        public string Image { get; set; }

        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }

    }
}

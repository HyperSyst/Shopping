using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class BrandModel
    {
        [Key]
        public int BrandId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên thương hiệu")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mô tả thương hiệu")]
        public string Description { get; set; }
        public string Slug {  get; set; }
        public int Status {  get; set; }
    }
}

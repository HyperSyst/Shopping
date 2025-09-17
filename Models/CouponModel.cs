using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class CouponModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mô tả!")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số lượng!")]
        public int Quantity {  get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int Status { get; set; }
    }
}

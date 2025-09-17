using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class ProductQuantityModel
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số lượng!")]
        public int Quantity { get; set; }
        public int ProductID { get; set; }
        public DateTime DateCreate { get; set; }
    }
}

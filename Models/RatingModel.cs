using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping.Models
{
    public class RatingModel
    {
        [Key]
        public int Id { get; set; }
        public int ProductID {  get; set; }

        public string Comment { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên!")]
        public string Name {  get; set; }
        [Required(ErrorMessage = "Vui lòng nhập email!")]
        public string Email {  get; set; }
        public string Rating {  get; set; }

        [ForeignKey("ProductID")]
        public ProductsModel Products { get; set; }
    }
}

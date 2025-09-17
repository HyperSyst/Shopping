using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping.Models
{
    public class CompareModel
    {
        [Key]
        public int Id { get; set; }
        public int ProductID { get; set; }
        public string UserID {  get; set; }

        [ForeignKey("ProductID")]

        public ProductsModel Products { get; set; }
    }
}

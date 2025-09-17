using Shopping.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping.Models
{
    public class ProductsModel
    {
        [Key]
        public int Id { get; set; }
        [Required, MinLength(4, ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        public string Name { get; set; }

        [Required, MinLength(4, ErrorMessage = "Vui lòng nhập mô tả sản phẩm")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá sản phẩm")]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập giá vốn")]
        public decimal CapitalPrice { get; set; }
        public string Slug {  get; set; }
        [Required, Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn thương hiệu sản phẩm")]
        public int BrandID {  get; set; }
        [Required, Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn danh mục sản phẩm")]
        public int CategoryID {  get; set; }
        [ForeignKey("CategoryID")]
        public CategoriesModel Categories { get; set; }
        [ForeignKey("BrandID")]
        public BrandModel Brand { get; set; }
        public string Image { get; set; }
        public int Quantity {  get; set; } 
        public int Sold { get; set; }

        public ICollection<RatingModel> Rating { get; set; } = new List<RatingModel>();
        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }
    }
}

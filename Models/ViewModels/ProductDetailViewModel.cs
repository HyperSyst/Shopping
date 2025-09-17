using System.ComponentModel.DataAnnotations;

namespace Shopping.Models.ViewModels
{
    public class ProductDetailViewModel
    {
        public ProductsModel Products { get; set; }
        public List<RatingModel> Rating { get; set; } = new ();
        public RatingModel Ratings { get; set; }
    }
}

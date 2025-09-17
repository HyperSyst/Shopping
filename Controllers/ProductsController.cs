using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Migrations;
using Shopping.Models;
using Shopping.Models.ViewModels;
using Shopping.Repository;

namespace Shopping.Controllers
{
    public class ProductsController : Controller
    {
        private readonly DataContext _dataContext;
        public ProductsController(DataContext context)
        {
            _dataContext = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Reviews(int id)
        {
            var product = _dataContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Categories)
                .Include(p => p.Rating) // Include để EF load các đánh giá
                .FirstOrDefault(p => p.Id == id);

            if (product == null) return NotFound();

            var viewModel = new ProductDetailViewModel
            {
                Products = product,
                Rating = product.Rating.ToList()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int Id)
        {
            if (Id == null)
                return RedirectToAction("Index");
            var productsById = _dataContext.Products.Include(p => p.Rating).Where(c => c.Id == Id).FirstOrDefault();

            var relatedProducts = await _dataContext.Products
            .Where(p => p.CategoryID == productsById.CategoryID && p.Id != productsById.Id).Take(4).ToListAsync();
            ViewBag.RelatedProducts = relatedProducts;

            var viewModel = new ProductDetailViewModel
            {
                Products = productsById,
                Ratings = new RatingModel { ProductID = Id }
            };

            var listCmt = await _dataContext.Rating.Where(p => p.ProductID == Id).ToListAsync();
            var brandInfo = _dataContext.Brands.Where(p => p.BrandId == productsById.BrandID).FirstOrDefault();
            ViewBag.listCmt = listCmt;
            ViewBag.brandInfo = brandInfo.Description;

            return View(viewModel);
        }

        public async Task<IActionResult> CommentProduct(ProductDetailViewModel rating)
        {
            if (ModelState.IsValid)
            {
                var ratingEntity = new RatingModel
                {
                    ProductID = rating.Ratings.ProductID,
                    Name = rating.Ratings.Name,
                    Email = rating.Ratings.Email,
                    Comment = rating.Ratings.Comment,
                    Rating = rating.Ratings.Rating
                };

                _dataContext.Rating.Add(ratingEntity);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Thêm đánh giá thành công";
                return Redirect(Request.Headers["Referer"]);

            }
            else
            {
                rating.Products = _dataContext.Products
                                   .Include(p => p.Rating)
                                   .FirstOrDefault(p => p.Id == rating.Ratings.ProductID);

                return View("Detail", rating);
            }
        }
    }
}

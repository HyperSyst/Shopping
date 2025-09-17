using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;
using Shopping.Repository;

namespace Shopping.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUserModel> _userManager;

        public HomeController(ILogger<HomeController> logger, DataContext context, UserManager<AppUserModel> userManager)
        {
            _logger = logger;
            _dataContext = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var products = _dataContext.Products.Include("Categories").Include("Brand").ToList();
            return View(products);
        }

        public async Task<IActionResult> Contact()
        {
            var contact = await _dataContext.Contacts.FirstAsync(); 

            return View(contact);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statuscode)
        {
            if(statuscode == 404)
            {
                return View("NotFound");
            }
            else
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public async Task<IActionResult> Compare()
        {
            var compareProduct = await (from c in _dataContext.Compares
                                        join p in _dataContext.Products on c.ProductID equals p.Id
                                        join u in _dataContext.Users on c.UserID equals u.Id
                                        select new {User = u, Product = p, Compare = c}).ToListAsync();
            return View(compareProduct);
        }

        public async Task<IActionResult> Wishlist()
        {
            var wishlistProduct = await (from w in _dataContext.Wishlists
                                         join p in _dataContext.Products on w.ProductID equals p.Id
                                         join u in _dataContext.Users on w.UserID equals u.Id
                                         select new { User = u, Wishlist = w, Product = p }).ToListAsync();
            return View(wishlistProduct);
        }
        public async Task<IActionResult> AddWishlist(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var wishlistProduct = new WishlistModel
            {
                ProductID = id,
                UserID = user.Id
            }; 
            _dataContext.Wishlists.Add(wishlistProduct);

            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { succes = true, message = "Thêm thành công!" });
            }
            catch (Exception)
            {
                return StatusCode(500, "Đã có lỗi xảy ra!");
            }
        }

        public async Task<IActionResult> AddCompare(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var compareProduct = new CompareModel
            {
                ProductID = id,
                UserID = user.Id
            };
            _dataContext.Compares.Add(compareProduct);

            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { succes = true, message = "Thêm thành công!" });
            }
            catch (Exception)
            {
                return StatusCode(500, "Đã có lỗi xảy ra!");
            }
        }

        public async Task<IActionResult> DeleteCompare(int Id)
        {
            CompareModel compare = await _dataContext.Compares.FindAsync(Id);

            _dataContext.Compares.Remove(compare);

            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Xóa thành công";
            return RedirectToAction("Compare", "Home");
        }

        public async Task<IActionResult> DeleteWishlist(int Id)
        {
            WishlistModel wishlist = await _dataContext.Wishlists.FindAsync(Id);

            _dataContext.Wishlists.Remove(wishlist);

            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Xóa thành công";
            return RedirectToAction("Wishlist", "Home");
        }
    }
}

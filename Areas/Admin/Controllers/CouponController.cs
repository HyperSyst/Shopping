using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;
using Shopping.Repository;

namespace Shopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]

    public class CouponController : Controller
    {
        private readonly DataContext _dataContext;
        public CouponController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index()
        {
            var couponList = await _dataContext.Coupon.ToListAsync();
            ViewBag.Coupon = couponList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CouponModel coupon)
        {

            if (ModelState.IsValid)
            {
                _dataContext.Add(coupon);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Coupon đã được thêm";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Model lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMess = string.Join("\n", errors);
                return BadRequest(errorMess);
            }
        }

    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;
using Shopping.Repository;
using System.Security.Cryptography;

namespace Shopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")] 
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;
        public BrandController(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<IActionResult> Index(int pg = 1)
        {
            List<BrandModel> brand = _dataContext.Brands.ToList();
            const int pageSize = 10;

            if (pg < 1)
            {
                pg = 1;
            }
            int recsCount = brand.Count();
            var pager = new Paginate(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = brand.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewBag.Pager = pager;
            return View(data);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandModel brand)
        {

            if (ModelState.IsValid)
            {
                brand.Slug = brand.Name.Replace(" ", "-");
                var slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == brand.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Thương hiệu đã có trong database");
                    return View(brand);
                }

                _dataContext.Add(brand);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thương hiệu đã được thêm";
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
            return View(brand);
        }

        public async Task<IActionResult> Edit(int id)
        {
            BrandModel brand = await _dataContext.Brands.FindAsync(id);

            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BrandModel brand)
        {

            if (ModelState.IsValid)
            {
                brand.Slug = brand.Name.Replace(" ", "-");
                var slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == brand.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Thương hiệu đã có trong database");
                    return View(brand);
                }

                _dataContext.Update(brand);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thương hiệu đã được cập nhật";
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
            return View(brand);
        }

        public async Task<IActionResult> Delete(int id)
        {
            BrandModel brand = await _dataContext.Brands.FindAsync(id);

            _dataContext.Brands.Remove(brand);
            await _dataContext.SaveChangesAsync();
            TempData["Error"] = "Đã xóa danh mục!";
            return RedirectToAction("Index");
        }

    }
}

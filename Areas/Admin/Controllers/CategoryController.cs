using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;
using Shopping.Repository;

namespace Shopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<IActionResult> Index(int pg = 1)
        {
            List<CategoriesModel> category = _dataContext.Categories.ToList();
            const int pageSize = 10;    

            if (pg < 1)
            {
                pg = 1;
            }
            int recsCount = category.Count();
            var pager = new Paginate(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = category.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewBag.Pager = pager;
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            CategoriesModel categories = await _dataContext.Categories.FindAsync(id);

            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoriesModel category)
        {

            if (ModelState.IsValid)
            {
                category.Slug = category.Name.Replace(" ", "-");
                var slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug && p.CategoryId != category.CategoryId);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Danh mục đã có trong database");
                    return View(category);
                }

                _dataContext.Update(category);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Danh mục đã được cập nhật";
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
            return View(category);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoriesModel category)
        {

            if (ModelState.IsValid)
            {
                category.Slug = category.Name.Replace(" ", "-");
                var slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Danh mục đã có trong database");
                    return View(category);
                }

                _dataContext.Add(category);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Danh mục đã được thêm";
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
            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
            CategoriesModel categories = await _dataContext.Categories.FindAsync(id);

            _dataContext.Categories.Remove(categories);
            await _dataContext.SaveChangesAsync();
            TempData["Error"] = "Đã xóa danh mục!";
            return RedirectToAction("Index");
        }

    }
}

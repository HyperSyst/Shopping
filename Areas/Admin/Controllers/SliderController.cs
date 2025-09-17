using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Shopping.Models;
using Shopping.Repository;
using System.Runtime.InteropServices;


namespace Shopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Contact")]
    [Authorize(Roles = "admin")]
    public class SliderController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SliderController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(int pg = 1)
        {
            return View(await _dataContext.Sliders.OrderByDescending(p => p.Id).ToListAsync());
        }

        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderModel slider)
        {
            if (ModelState.IsValid)
            {
                if (slider.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/sliders");
                    string imgName = Guid.NewGuid().ToString() + "_" + slider.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imgName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await slider.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    slider.Image = imgName;
                }
                _dataContext.Add(slider);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Slider đã được thêm";
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

        [Route("Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            SliderModel slider = await _dataContext.Sliders.FindAsync(id);
            return View(slider);
        }
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SliderModel slider)
        {
            var existedSlider = _dataContext.Sliders.Find(slider.Id);
            if (ModelState.IsValid)
            {
                if (slider.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/sliders");
                    string imgName = Guid.NewGuid().ToString() + "_" + slider.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imgName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await slider.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    existedSlider.Image = imgName;
                }

                existedSlider.Name = slider.Name;
                existedSlider.Description = slider.Description;
                existedSlider.Status = slider.Status;

                _dataContext.Update(existedSlider);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Slider đã được cập nhật";
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

        [Route("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            SliderModel slider = await _dataContext.Sliders.FindAsync(id);
            if (slider == null)
            {
                return NotFound();
            }

            string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/sliders");
            string oldFileImage = Path.Combine(uploadDir, slider.Image);

            if (!string.IsNullOrEmpty(slider.Image))
            {
                try
                {
                    if (System.IO.File.Exists(oldFileImage))
                    {
                        System.IO.File.Delete(oldFileImage);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while deleting the slider image.");
                }
            }
            _dataContext.Sliders.Remove(slider);
            await _dataContext.SaveChangesAsync();
            TempData["Error"] = "Đã xóa slider!";
            return RedirectToAction("Index");
        }

    }
}

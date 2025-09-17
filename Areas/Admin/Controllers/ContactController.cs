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
    public class ContactController : Controller 
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ContactController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("Index")]
        public IActionResult Index()
        {
            var contact = _dataContext.Contacts.ToList();
            return View(contact);
        }

        [Route("Edit")]
        public async Task<IActionResult> Edit()
        {
            ContactModel contact = await _dataContext.Contacts.FirstOrDefaultAsync();
            return View(contact);
        }

        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ContactModel contact)
        {
            var existed_contact = _dataContext.Contacts.FirstOrDefault();

            if (ModelState.IsValid)
            {
                if (contact.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/logo");
                    string imgName = Guid.NewGuid().ToString() + "_" + contact.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imgName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await contact.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    existed_contact.Image = imgName;
                }

                existed_contact.Name = contact.Name;
                existed_contact.Description = contact.Description;
                existed_contact.Phone = contact.Phone;
                existed_contact.Email = contact.Email;
                existed_contact.Map = contact.Map;

                _dataContext.Update(existed_contact);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Sản phẩm đã được cập nhật!";
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

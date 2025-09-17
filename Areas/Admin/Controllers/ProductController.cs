using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;
using Shopping.Repository;

namespace Shopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(int pg = 1)
        {
            List<ProductsModel> product = _dataContext.Products.OrderByDescending(p => p.Id).Include(p => p.Brand).Include(p => p.Categories).ToList();
            const int pageSize = 10;

            if (pg < 1)
            {
                pg = 1;
            }
            int recsCount = product.Count();
            var pager = new Paginate(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = product.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewBag.Pager = pager;
            return View(data);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "CategoryId", "Name");
            ViewBag.Brands = new SelectList(_dataContext.Brands, "BrandId", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductsModel product)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "CategoryId", "Name", product.CategoryID);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "BrandId", "Name", product.BrandID);

            if(ModelState.IsValid)
            {
                product.Slug = product.Name.Replace(" ", "-");
                var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
                if (slug != null) 
                {
                    ModelState.AddModelError("", "Sản phẩm đã có trong database");
                    return View(product);
                }
                if (product.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string imgName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imgName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    product.Image = imgName;
                }
                _dataContext.Add(product);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Sản phẩm đã được thêm";
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

        public async Task<IActionResult> Edit(int id)
        {
            ProductsModel product = await _dataContext.Products.FindAsync(id);
            ViewBag.Categories = new SelectList(_dataContext.Categories, "CategoryId", "Name", product.CategoryID);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "BrandId", "Name", product.BrandID);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductsModel product, int id)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "CategoryId", "Name", product.CategoryID);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "BrandId", "Name", product.BrandID);

            var existed_product = _dataContext.Products.Find(product.Id);

            if (ModelState.IsValid)
            {
                product.Slug = product.Name.Replace(" ", "-");
                var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug && p.Id != id);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Sản phẩm đã có trong database");
                    return View(product);
                }
                if (product.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string imgName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imgName);

                    if (!string.IsNullOrEmpty(existed_product.Image))
                    {
                        string oldFileImage = Path.Combine(uploadDir, existed_product.Image);
                        try
                        {
                            if (System.IO.File.Exists(oldFileImage))
                            {
                                System.IO.File.Delete(oldFileImage);
                            }
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "An error occurred while deleting the product image.");
                        }
                    }
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    existed_product.Image = imgName;
                }

                existed_product.Name = product.Name;
                existed_product.Description = product.Description;
                existed_product.Price = product.Price;
                existed_product.CapitalPrice = product.CapitalPrice;
                existed_product.CategoryID = product.CategoryID;    
                existed_product.BrandID = product.BrandID;

                _dataContext.Update(existed_product);
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

        public async Task<IActionResult> Delete(int id)
        {
            ProductsModel product = await _dataContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
            string oldFileImage = Path.Combine(uploadDir, product.Image);

            if (!string.IsNullOrEmpty(product.Image))
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
                    ModelState.AddModelError("", "An error occurred while deleting the product image.");
                }
            }
            _dataContext.Products.Remove(product);
            await _dataContext.SaveChangesAsync();
            TempData["Error"] = "Đã xóa sản phẩm!";
            return RedirectToAction("Index");
        }

        //Add quantity
        [HttpGet]
        public async Task<IActionResult> AddQuantity(int Id)
        {
            var productQuantity = await _dataContext.Quantity.Where(pq => pq.ProductID == Id).ToListAsync();
            ViewBag.Quantity = productQuantity;
            ViewBag.ProductID = Id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StoreProductQuantity(ProductQuantityModel productQuantityModel)
        {
            // Get the product to update
            var product = _dataContext.Products.Find(productQuantityModel.ProductID);

            if (product == null)
            {
                return NotFound(); // Handle product not found scenario
            }
            product.Quantity += productQuantityModel.Quantity;

            productQuantityModel.Quantity = productQuantityModel.Quantity;
            productQuantityModel.ProductID = productQuantityModel.ProductID;
            productQuantityModel.DateCreate = DateTime.Now;


            _dataContext.Add(productQuantityModel);
            _dataContext.SaveChanges();
            TempData["success"] = "Thêm số lượng sản phẩm thành công";
            return RedirectToAction("AddQuantity", "Product", new { Id = productQuantityModel.ProductID });
        }
    }
}

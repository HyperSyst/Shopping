using Microsoft.EntityFrameworkCore;
using Shopping.Models;

namespace Shopping.Repository
{
    public class SeedData
    {
        public static void SeedingData(DataContext _context)
        {
            _context.Database.Migrate();
            if (!_context.Products.Any())
            {
                CategoriesModel macbook = new CategoriesModel {Name = "Macbook", Slug = "macbook", Description = "Macbook is a large product in the world", Status = 1};
                CategoriesModel pc = new CategoriesModel { Name = "Pc", Slug = "pc", Description = "PC is a large product in the world", Status = 1 };

                BrandModel samsung = new BrandModel { Name = "Samsung", Slug = "samsung", Description = "Samsung is a large brand in the world", Status = 1 };
                BrandModel apple = new BrandModel { Name = "Apple", Slug = "apple", Description = "Apple is a large brand in the world", Status = 1 };
                _context.Products.AddRange(
                    new ProductsModel { Name = "Macbook", Slug = "macbook", Description = "Macbook is the best", Image = "1.jpg", Categories = macbook, Brand = apple, Price = 1299 },
                    new ProductsModel { Name = "Pc", Slug = "pc", Description = "Pc is the best", Image = "1.jpg", Categories = pc, Brand = samsung, Price = 1500 }
                );
                _context.SaveChanges();
            }
        }
    }
}

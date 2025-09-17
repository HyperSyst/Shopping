using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;

namespace Shopping.Repository
{
    public class DataContext : IdentityDbContext<AppUserModel>
    {
        public DataContext(DbContextOptions<DataContext> options) : base( options ) { }
        public DbSet<BrandModel> Brands { get; set; }
        public DbSet<ProductsModel> Products { get; set; }
        public DbSet<CategoriesModel> Categories { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<RatingModel> Rating { get; set; }
        public DbSet<SliderModel> Sliders { get; set; }
        public DbSet<ContactModel> Contacts { get; set; }
        public DbSet<WishlistModel> Wishlists { get; set; }
        public DbSet<CompareModel> Compares { get; set; }
        public DbSet<ProductQuantityModel> Quantity { get; set; }
        public DbSet<ShippingModel> Shipping { get; set; }
        public DbSet<CouponModel> Coupon { get; set; }
        public DbSet<StatisticalModel> Statistical { get; set; }
        public DbSet<VnpayModel> Vnpay { get; set; }

    }
}
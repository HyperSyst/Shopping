using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shopping.Areas.Admin.Repository;
using Shopping.Models;
using Shopping.Models.Momo;
using Shopping.Repository;
using Shopping.Services.Momo;
using Shopping.Services.VnPay;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IVnPayService, VnPayService>();

builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
builder.Services.AddScoped<IMomoService, MomoServices>();

// Connection db
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DbConnect"]);
});
//Add Email Sender
builder.Services.AddTransient<IEmailSender, EmailSender>();
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromMinutes(15);
    opt.Cookie.IsEssential = true;
});

builder.Services.AddIdentity<AppUserModel,IdentityRole>()
    .AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
});

builder.Services.AddAuthentication(option =>
{
    //option.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //option.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //option.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie().AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
    options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
});

var app = builder.Build();

app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "Areas",
    pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "Categories",
    pattern: "/category/{Slug?}",
    defaults: new {controller="Categories", action="Index"});

app.MapControllerRoute(
    name: "Brand",
    pattern: "/brand/{Slug?}",
    defaults: new { controller = "Brand", action = "Index" });

app.MapControllerRoute(
    name: "Order",
    pattern: "/order/{Slug?}",
    defaults: new { controller = "Order", action = "Index" });

app.MapControllerRoute(
    name: "User",
    pattern: "/user/{Slug?}",
    defaults: new { controller = "User", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
SeedData.SeedingData(context);

app.Run();

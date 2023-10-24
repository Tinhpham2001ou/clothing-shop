using ClothingShop.WEB.Middleware;
using ClothingShop.WEB.Models;
using ClothingShop.WEB.Utils.CloudinaryService;
using ClothingShop.WEB.Utils.Email;
using ClothingShop.WEB.Utils.UnitOfWork;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession();
builder.Services.AddDbContext<ClothingShopContext>(options => options.UseSqlServer(builder.Configuration
    .GetConnectionString("Database")));

builder.Services.AddScoped<IEmail, SMTPUtil>();
builder.Services.AddScoped<IUploadImage, CloudinaryUtil>();
builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.UseMiddleware<AuthorizedMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();

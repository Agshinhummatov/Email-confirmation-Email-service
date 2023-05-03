using Elearn_temp.Data;
using Elearn_temp.Helpers;
using Elearn_temp.Models;
using Elearn_temp.Services;
using Elearn_temp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddSession(); //bunu bele qeyd edirikki yeni burada session storage istifade edeceyik


builder.Services.AddDbContext<AppDbContext>(option =>
{

    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders(); /// bunu login registr ucun yazriq


builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 8; // bu paswordun lensi en azi 8 olmalidir yeni girilen sifreye en azi 8 simvol daxil elemlidir
    options.Password.RequireDigit = true; // passworda reqem mutleq sekilde olsun
    options.Password.RequireLowercase = true; // balaca herifler mutlreq sekilde olsun
    options.Password.RequireUppercase = true; // boyuk herif en azi 1 dene olsun
    options.Password.RequireNonAlphanumeric = true;  // simvolar en azi 1 dene oslun yeni herif ve reqemden basqa  altdan xet meselcun noqte ve s

    options.User.RequireUniqueEmail = true; // her istifadeci ucun bir emale olmalidir yeni bir emailden 2 istifadeci istifade edib registir ola bilmez

    options.SignIn.RequireConfirmedEmail = true;
    options.Lockout.MaxFailedAccessAttempts = 3; // 3 defe   logini tekrar tekerar kece  biler en azi 3 defe sehv ede biler

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);  // bu ise 2 defe sehv edenden sonra bloklayir 30 deqiqelik

    options.Lockout.AllowedForNewUsers = true; // bu ise odurki yeni registerden kecen adam en azi 1 defe login olmalidir yuxarida yazilanlar ona ait deyil yeni sehv ede biler

});


builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();   //servicelerin brauserdeki(cookiedeki) datalara el chatanligini temin etm,ek uchun

builder.Services.AddScoped<ICourseService, CourseService>();  //LayoutServiceni istifade etmek uchun 

builder.Services.AddScoped<IAuthorService, AuthorService>();  //LayoutServiceni istifade etmek uchun 

builder.Services.AddScoped<IEmailService, EmailService>();


builder.Services.AddScoped<EmailSettings>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles(); //static fayllar uchun yeni jc css falan

app.UseSession(); // session istifade edeceyik

app.UseRouting();

app.UseAuthentication(); // login edende ede bilsin deye yaziriq

app.UseAuthorization();


app.MapControllerRoute(
    name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

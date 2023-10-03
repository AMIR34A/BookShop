using BookShop.Areas.Admin.Services;
using BookShop.Areas.Identity.Data;
using BookShop.Data;
using BookShop.Models;
using BookShop.Models.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using ReflectionIT.Mvc.Paging;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("IdentityContextConnection") ?? throw new InvalidOperationException("Connection string 'IdentityContextConnection' not found.");

builder.Services.AddDbContext<IdentityContext>(options =>
    options.UseSqlServer(connectionString));

//builder.Services.AddDefaultIdentity<BookShopUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<IdentityContext>();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddDefaultUI()
    .AddEntityFrameworkStores<IdentityContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<ApplicationIdentityErrorDescriber>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequireLowercase = true;

    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$";

    options.SignIn.RequireConfirmedEmail = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<BookShopContext>();
builder.Services.AddTransient<BooksRepository>();
builder.Services.AddTransient<PersianCalendar>();
builder.Services.AddScoped<IApplicationRoleManager, ApplicationRoleManager>();
builder.Services.AddScoped<IApplicationUserManager, ApplicationUserManager>();
builder.Services.AddScoped<ApplicationIdentityErrorDescriber>();
builder.Services.AddScoped<IEmailSender, EmailSenderService>();

builder.Services.AddLocalization(option => option.ResourcesPath = "Resources");
builder.Services.AddMvc(setup =>
{
    var factory = builder.Services.BuildServiceProvider().GetService<IStringLocalizerFactory>();
    var localizer = factory.Create("ModelBindingMessages", "BookShop");
    setup.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(value => localizer["این فیلد باید پر شود"]);
});

builder.Services.AddPaging(option =>
{
    option.ViewName = "Bootstrap4";
    option.HtmlIndicatorDown = "<i class='fa fa-sort-amount-down'></i>";
    option.HtmlIndicatorUp = "<i class='fa fa-sort-amount-up'></i>";
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseDeveloperExceptionPage();
    //app.UseStaticFiles(new StaticFileOptions
    //{
    //    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "node_modules")),
    //    RequestPath = "/" + "node_modules",
    //});
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();



app.MapAreaControllerRoute(
name: "AdminArea",
areaName: "Admin",
pattern: "Admin/{controller}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


//app.MapControllerRoute(
//    name: "Route1",
//    pattern: "Home/{action}/{id?}",
//    defaults: new { Controller = "Book", Action = "Index" });

app.Run();

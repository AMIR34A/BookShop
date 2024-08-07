using Asp.Versioning;
using BookShop.Areas.Admin.Data;
using BookShop.Areas.Admin.Services;
using BookShop.Areas.API.Classes;
using BookShop.Areas.API.Middlewares;
using BookShop.Areas.API.Services;
using BookShop.Areas.API.Swagger;
using BookShop.Areas.Identity.Data;
using BookShop.Classes;
using BookShop.Data;
using BookShop.Exceptions;
using BookShop.Models;
using BookShop.Models.Repository;
using BookShop.Policies;
using BookShop.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using ReflectionIT.Mvc.Paging;
using System.Globalization;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("IdentityContextConnection") ?? throw new InvalidOperationException("Connection string 'IdentityContextConnection' not found.");

builder.Services.AddDbContext<IdentityContext>(options =>
    options.UseSqlServer(connectionString));

//builder.Services.AddDefaultIdentity<BookShopUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<IdentityContext>();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
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

    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);
    options.Lockout.MaxFailedAccessAttempts = 3;
});

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = TimeSpan.FromMinutes(20);
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader(), new HeaderApiVersionReader());
});
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<BookShopContext>();
builder.Services.AddTransient<BooksRepository>();
builder.Services.AddTransient<PersianCalendar>();
builder.Services.AddTransient<IJwtService, JwtService>();
builder.Services.AddScoped<IApplicationRoleManager, ApplicationRoleManager>();
builder.Services.AddScoped<IApplicationUserManager, ApplicationUserManager>();
builder.Services.AddScoped<ApplicationIdentityErrorDescriber>();
builder.Services.AddScoped<IEmailSender, EmailSenderService>();
builder.Services.AddScoped<ISMSSenderService, SMSSenderService>();
builder.Services.AddSingleton<IAuthorizationHandler, DateOfBirthAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, DynamicPermissionAuthorizationHandler>();
builder.Services.AddSingleton<IMVCActionsDiscoveryService, MVCActionsDiscoveryService>();
builder.Services.AddSingleton<ISecurityTrimmingService, SecurityTrimmingService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddSwagger();

builder.Services.Configure<SiteSettings>(builder.Configuration.GetSection(nameof(SiteSettings)));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    SiteSettings? siteSettings = builder.Configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();
    byte[] secretKey = Encoding.UTF8.GetBytes(siteSettings.JWTSettings.SecretKey);

    byte[] bytes = Encoding.UTF8.GetBytes(siteSettings.JWTSettings.EncryptKey);

    TokenValidationParameters tokenValidationParameters = new TokenValidationParameters()
    {
        RequireSignedTokens = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        RequireExpirationTime = true,
        ValidateLifetime = true,
        ValidateAudience = true,
        ValidAudience = siteSettings.JWTSettings.Audience,
        ValidateIssuer = true,
        ValidIssuer = siteSettings.JWTSettings.Issuer,
        TokenDecryptionKey = new SymmetricSecurityKey(bytes)
    };
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = tokenValidationParameters;
    options.Events = new JwtBearerEvents()
    {
        OnAuthenticationFailed = context =>
        {
            return context.Exception is not null ? throw new AppException(ApiResultStatusCode.UnAuthorized, "Authentication failed", HttpStatusCode.Unauthorized, context.Exception) : Task.CompletedTask;
        },

        OnTokenValidated = async context =>
        {
            var userRepository = context.HttpContext.RequestServices.GetRequiredService<IApplicationUserManager>();
            var claimsIdentity = context.Principal.Claims;
            if (claimsIdentity.Count() == 0)
                context.Fail("This token has no claims");
            var securityStamp = claimsIdentity.FirstOrDefault(c => c.Type.Equals("SecurityStampClaimType"));
            if (securityStamp is null)
                context.Fail("This token has no security stamp");
            var user = await userRepository.GetUserAsync(context.Principal);
            if (user.SecurityStamp != securityStamp.Value)
                context.Fail("Token security stamp is invalid");
            if (!user.IsActive)
                context.Fail("User is not active");
        }
    };
})
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration.GetSection("ExternalLogIn")["ClientId"];
        options.ClientSecret = builder.Configuration.GetSection("ExternalLogIn")["ClientSecret"];
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AccessToUserManager", policy => policy.RequireRole("مدیر سایت"));
    options.AddPolicy("DateOfBirth", policy => policy.Requirements.Add(new DateOfBirthAuthorizationRequirement()));
    options.AddPolicy("NeedMinimumAge", policy => policy.AddRequirements(new MinimumAgeAuthorizationRequirement(18)));
    options.AddPolicy(ConstantPolicies.DynamicPermission, policy => policy.AddRequirements(new DynamicPermissionAuthorizationRequirement()));
});

builder.Services.ConfigureApplicationCookie(configure =>
{
    configure.LoginPath = "/Account/SignIn";
});

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

app.CustomExceptionHandler();
//app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.UseSession();
app.AddSwaggerAndSwaggerUI();

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

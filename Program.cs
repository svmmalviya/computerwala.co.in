using Serilog;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using computerwala.MWs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using DBService.Repositories;
using DBService.Interfaces;
using DBService.AppContext;
using Microsoft.EntityFrameworkCore;
using computerwala.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();
builder.Services.AddSerilog();
builder.Services.AddSession();
builder.Services.AddMvc(o => { o.EnableEndpointRouting = false;

});


var connectionStr = builder.Configuration.GetConnectionString("CWConnection");
builder.Services.AddDbContext<AppDBContext>(o => { o.UseSqlServer(connectionStr); });



builder.Services.AddAuthentication(options =>

{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = "localhost:7037",
        ValidAudience = "localhost:7037",
        ClockSkew=TimeSpan.FromMinutes(60),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("website_computerwala.co.in.indianWebsite"))
    };
});

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//}).AddJwtBearer(options =>
//{

//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = false,
//        ValidateAudience = false,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        //ValidIssuer = "https://localhost:7006", // Replace with your issuer
//        //ValidAudience = "https://localhost:7006", // Replace with your audience
//        ClockSkew = TimeSpan.FromMinutes(1),
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("sdfdslkfjdskljfldsjklfjsldkjflksdjlfjlsdjfljsdlkfjlksdjfljslkdjfldskj")) // Replace with your secret key
//    };
//});
builder.Services.AddCors((options) =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            // Create a new HttpContextAccessor
            var httpContextAccessor = new HttpContextAccessor();

            // Set the HttpContextAccessor.HttpContext to the current HttpContext
            // This line is essential for getting the current context
            httpContextAccessor.HttpContext = new DefaultHttpContext();

            // Now you can access the current HttpContext
            HttpContext currentHttpContext = httpContextAccessor.HttpContext;

            builder.WithOrigins($"{currentHttpContext.Request.Scheme}://{currentHttpContext.Request.Host}")
                   .WithHeaders("content-security-policy,cache-control")
                   .WithMethods("get,post");
        });
});

builder.Services.AddHttpContextAccessor();
#region DIs
builder.Services.AddScoped<MaintenanceController>();
builder.Services.AddTransient<IAuthentication, Authentication>();
builder.Services.AddTransient<ICWSubscription, CWSubscription>();
builder.Services.AddTransient<MaintenanceMW>();
#endregion

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseCors("AllowSpecificOrigins");

// Create a configuration object to read from appsettings.json
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// Configure Serilog using the settings from appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

Log.Information("Application started");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseMvc(o =>
{
	o.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
	o.MapRoute("admin", "Sophisticated/{controller=Admin}/{action=Index}/{id?}");
});

app.Use(async (context, next) =>
{
	var maintenanceEnabled = app.Configuration["Maintenance"];

	Log.Logger.Information("In Maintenance enabled: " + maintenanceEnabled);
	if (maintenanceEnabled.ToLower() == "enable")
	{
		Log.Logger.Information("Redirecting to maintenance page");
		context.Response.Redirect("/Maintenance/Index", true);
		//context.Request.Path = "/Maintenance/Index";
		return;
	}

	await next();
});
app.UseAuthorization();

app.UseSession();
//app.UseMiddleware<MaintenanceMW>();

//app.UseMiddleware<JwtTokenCacheMiddleware>();






//app.MapDefaultControllerRoute();
//app.UseEndpoints(endpoints =>
//{
//	endpoints.MapControllerRoute(
//		name: "Admin",
//		pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");

//	// Define your main application routes here.
//	endpoints.MapControllerRoute(
//		name: "default",
//		pattern: "{controller=Home}/{action=Index}/{id?}");
//});


app.Run();

using Serilog;
using computerwala.MWs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DBService.Repositories;
using DBService.Interfaces;
using DBService.AppContext;
using computerwala.Controllers;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();
builder.Services.AddSerilog();
builder.Services.AddSession();
builder.Services.AddMvc(o =>
{
	o.EnableEndpointRouting = false;

});



builder.Services.AddDbContext<AppDBContext>(o =>
{


	var connectionStr = "";

	if (builder.Configuration["dbtype"].ToLower() == "mysql")
	{
		connectionStr = builder.Configuration.GetConnectionString("MySqlConnection");
		o.UseMySQL(connectionStr);
	}
	else
	{
		connectionStr = builder.Configuration.GetConnectionString("SqlServerConnection");
		o.UseSqlServer(connectionStr);
	}

});



builder.Services.AddAuthentication(options =>

{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
	var httpAccessor = new HttpContextAccessor();
	httpAccessor.HttpContext = new DefaultHttpContext();
	var currentHttpContext = httpAccessor.HttpContext;

	options.TokenValidationParameters = new TokenValidationParameters
	{

		ValidateIssuer = true,
		ValidateAudience = true,
		ValidIssuer = $"{currentHttpContext.Request.Scheme}://{currentHttpContext.Request.Host}",
		ValidAudience = $"{currentHttpContext.Request.Scheme}://{currentHttpContext.Request.Host}",
		ClockSkew = TimeSpan.FromMinutes(60),
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
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<MaintenanceController>();
builder.Services.AddTransient<IAuthentication, Authentication>();
builder.Services.AddTransient<ICWEvent, CWEvent>();
builder.Services.AddTransient<ICWCalender,CWCalender>();
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
app.UseAuthorization();
app.UseSession();



app.UseMvc(o =>
{
	o.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
	o.MapRoute("admin", "Sophisticated/{controller=Admin}/{action=Index}/{id?}");
});

//app.UseMvc();


app.Use(async (context, next) =>
{
	var maintenanceenabled = Convert.ToBoolean(app.Configuration["maintenance"]);

	Log.Logger.Information("in maintenance enabled: " + maintenanceenabled);
	if (maintenanceenabled == true)
	{
		Log.Logger.Information("redirecting to maintenance page");
		context.Response.Redirect("/maintenance/index", true);
		//context.request.path = "/maintenance/index";
		return;
	}

	await next();
});
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

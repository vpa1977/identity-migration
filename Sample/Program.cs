using Microsoft.EntityFrameworkCore;
using Sample.Data;
using Sample.Hydra;
using Sample.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


// + HYDRA
builder.Services.Configure<HydraConfig>(builder.Configuration.GetSection("Hydra"));
builder.Services.AddTransient<HydraClient>();
builder.Services.AddTransient<OIDCConfigurationService>();

// -HYDRA

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddOpenId();

builder.Services.AddAuthentication();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();


{
    using var scope = app.Services.CreateScope();
    var adminClient = scope.ServiceProvider.GetRequiredService<HydraClient>();
    adminClient.DeleteOpenIdClient();
    adminClient.CreateOpenIdClient();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");
app.MapRazorPages();

app.MapFallbackToFile("index.html");;

app.Run();

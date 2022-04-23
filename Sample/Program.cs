using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
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

BuildIdentity(builder);

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

// + Allow cors since it is a sample without reverse proxy
app.UseCors(builder =>
{
    builder.AllowAnyOrigin();
});
// 

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapRazorPages();

app.MapFallbackToFile("index.html"); ;

app.Run();

static void BuildIdentity(WebApplicationBuilder builder)
{
    var hc = new HydraConfig();
    builder.Configuration.GetSection("Hydra").Bind(hc);
    builder.Services.AddAuthentication(o =>
    {
        o.DefaultScheme = IdentityConstants.ApplicationScheme;
        o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
        .AddJwtBearer(options =>
        {
            options.Authority = hc.PublicUrl;
            options.RequireHttpsMetadata = false; /// SAMPLE
            options.Audience = hc.ClientID;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = hc.PublicUrl,
                ValidAudience = hc.ClientID,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero
            };
        })
       /* .AddOpenIdConnect( o =>
        {
            o.Authority = hc.PublicUrl;
            o.RequireHttpsMetadata = false; // sample
            o.ClientId = hc.ClientID;
            o.ClientSecret = hc.Secret;
            o.RequireHttpsMetadata = false;

            o.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
            o.ResponseMode = OpenIdConnectResponseMode.Fragment;
            o.ResponseType = OpenIdConnectResponseType.Code;
        })*/
        .AddIdentityCookies();

    builder.Services.AddIdentityCore<ApplicationUser>(o =>
    {
        o.Stores.MaxLengthForKeys = 128;
        o.SignIn.RequireConfirmedAccount = true;
    })
        .AddRoles<IdentityRole>()
        .AddUserManager<UserManager<ApplicationUser>>()
        .AddSignInManager<SignInManager<ApplicationUser>>()
        .AddDefaultTokenProviders()
        .AddEntityFrameworkStores<ApplicationDbContext>();
}
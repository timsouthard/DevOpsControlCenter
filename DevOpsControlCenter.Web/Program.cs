using DevOpsControlCenter.Domain.Entities;
using DevOpsControlCenter.Infrastructure.Identity;
using DevOpsControlCenter.Infrastructure.Persistence;
using DevOpsControlCenter.Web.Components;
using DevOpsControlCenter.Web.Dtos;
using DevOpsControlCenter.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database (pointing at Infrastructure DbContext)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Lockout.MaxFailedAccessAttempts = 5;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".DevOpsControlCenter.Auth";
    options.Cookie.HttpOnly = true;

#if DEBUG
    options.Cookie.SameSite = SameSiteMode.Lax;  // Allow HttpClient to see it in dev
#else
    options.Cookie.SameSite = SameSiteMode.Strict;
#endif

    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.LoginPath = "/account/login";
    options.LogoutPath = "/account/logout";
    options.AccessDeniedPath = "/account/denied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);

     options.Events = new CookieAuthenticationEvents
    {
        OnSigningIn = ctx =>
        {
            Console.WriteLine("?? Cookie OnSigningIn fired");
            return Task.CompletedTask;
        },
        OnSignedIn = ctx =>
        {
            Console.WriteLine("?? Cookie OnSignedIn fired");
            return Task.CompletedTask;
        },
        OnValidatePrincipal = ctx =>
        {
            Console.WriteLine("?? Cookie OnValidatePrincipal fired");
            return Task.CompletedTask;
        }
    };
});

// Blazor + Auth
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options => { options.DetailedErrors = true; });


builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ApiClient>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var db = services.GetRequiredService<ApplicationDbContext>();

    if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
    {
        db.Database.Migrate();
    }

    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await IdentitySeeder.SeedAsync(userManager, roleManager);
}

app.MapPost("/api/account/login", async (
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    [FromForm] LoginRequest login) =>
{
    var user = await userManager.FindByEmailAsync(login.Email);
    if (user is not null && await userManager.CheckPasswordAsync(user, login.Password))
    {
        await signInManager.SignInAsync(user, isPersistent: login.RememberMe.HasValue ? login.RememberMe.Value : false);
        return Results.Redirect("/");
    }

    return Results.BadRequest(new { error = "Invalid login attempt." });
})
.DisableAntiforgery();

// Minimal API logout endpoint
app.MapPost("/api/account/logout", async (
    SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/"); // after logout, send them home
})
.DisableAntiforgery();


app.Run();
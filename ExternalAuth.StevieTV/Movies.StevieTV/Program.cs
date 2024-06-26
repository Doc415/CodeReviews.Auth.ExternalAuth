using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Movies.StevieTV.Data;
using Movies.StevieTV.Models;
using Microsoft.AspNetCore.Identity;
using Movies.StevieTV.Areas.Identity.Data;
using Movies.StevieTV.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MoviesContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MoviesContext") ?? throw new InvalidOperationException("Connection string 'MoviesContext' not found.")));

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MoviesContext")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationContext>();

builder.Services.AddScoped<ILogger, CustomLogger>();

// Add services to the container.
builder.Services.AddControllersWithViews();

//Add Authentication methods to the container
builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration.GetValue<string>("Authentication:Google:ClientId","") ?? throw new InvalidOperationException("Google ClientId Missing");
        googleOptions.ClientSecret = builder.Configuration.GetValue<string>("Authentication:Google:ClientSecret","") ?? throw new InvalidOperationException("Google ClientSecret Missing");
    })
    .AddFacebook(facebookOptions =>
    {
        facebookOptions.AppId = builder.Configuration.GetValue<string>("Authentication:Facebook:AppId") ?? throw new InvalidOperationException("Facebook AppId Missing");
        facebookOptions.AppSecret = builder.Configuration.GetValue<string>("Authentication:Facebook:AppSecret") ?? throw new InvalidOperationException("Facebook AppSecret Missing");
    })
    .AddGitHub(githubOptions =>
    {
        githubOptions.ClientId = builder.Configuration.GetValue<string>("Authentication:Github:ClientId","") ?? throw new InvalidOperationException("Github ClientId Missing");
        githubOptions.ClientSecret = builder.Configuration.GetValue<string>("Authentication:Github:ClientSecret","") ?? throw new InvalidOperationException("Github ClientSecret Missing");
    })
    .AddMicrosoftAccount(microsoftOptions =>
    {
        microsoftOptions.ClientId = builder.Configuration.GetValue<string>("Authentication:Microsoft:ClientId","") ?? throw new InvalidOperationException("Microsoft ClientId Missing");
        microsoftOptions.ClientSecret = builder.Configuration.GetValue<string>("Authentication:Microsoft:ClientSecret","") ?? throw new InvalidOperationException("Microsoft ClientSecret Missing");
    })
    .AddTwitter(twitterOptions =>
    {
        twitterOptions.ClientId = builder.Configuration.GetValue<string>("Authentication:Twitter:ClientId","") ?? throw new InvalidOperationException("Twitter ClientId Missing");
        twitterOptions.ClientSecret = builder.Configuration.GetValue<string>("Authentication:Twitter:ClientSecret","") ?? throw new InvalidOperationException("Twitter ClientSecret Missing");
    });


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Movies}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
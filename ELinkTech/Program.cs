using ELinkTech.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ELinkTech.Services;
using ELinkTech;

var builder = WebApplication.CreateBuilder(args);
var conString = builder.Configuration.GetConnectionString("ConnectionString");
// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(conString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => 
{
    options.SignIn.RequireConfirmedEmail = true;
})
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders()
    .AddRoles<IdentityRole>();

builder.Services.AddControllersWithViews();

// Add EmailSender
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Session management
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    /* app.UseExceptionHandler("/Main/Error");
     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
     app.UseHsts();*/
    app.UseExceptionHandler("/error.html");

    app.Use(async (context, next) =>
    {
        if (context.Request.Path.Value.Contains("invalid"))
            throw new Exception("ERROR!");

        await next();
    });
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting(); //Git ignore test

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=Index}/{id?}");

app.Run();

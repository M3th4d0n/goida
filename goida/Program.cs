using goida.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;
using System.Net.Http.Headers;
var builder = WebApplication.CreateBuilder(args);





builder.Services.AddHttpClient<GitHubService>(client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "C# App");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "thx github security<3");
});

builder.Services.AddMemoryCache(); // Добавляем IMemoryCache
builder.Services.AddControllersWithViews();



builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; 
        options.AccessDeniedPath = "/Account/AccessDenied"; 
    });


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true; 
});


builder.Services.AddDbContext<ChatContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("ChatDatabase"),
        new MySqlServerVersion(new Version(8, 0, 21))); 
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ChatContext>();
    dbContext.Database.EnsureCreated();
}


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication(); 
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

using LunchApp.Data;
using LunchApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Session (za login)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Data Protection – shrani ključe za session, da deluje tudi po redeployu
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"/app/keys")) // Docker / Render mapa
    .SetApplicationName("LunchApp");

// SQLite DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=lunchapp.db"));

// Servisi
builder.Services.AddSingleton<EmailService>();
builder.Services.AddHostedService<DailyReportService>();

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();      // ⚠️ PRED Authorization
app.UseAuthorization();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

// Ustvari DB ob zagonu
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Za Docker / Render: poskrbi, da mapa za ključe obstaja
if (!Directory.Exists("/app/keys"))
{
    Directory.CreateDirectory("/app/keys");
}

app.Run();

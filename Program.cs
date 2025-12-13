using LunchApp.Data;
using LunchApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ MVC
builder.Services.AddControllersWithViews();

// 2️⃣ Session (za login)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 3️⃣ Data Protection – shrani ključe za session
var keysPath = "/app/keys";
if (!Directory.Exists(keysPath))
{
    Directory.CreateDirectory(keysPath);
}
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
    .SetApplicationName("LunchApp");

// 4️⃣ SQLite DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=/app/lunchapp.db"));

// 5️⃣ Servisi (Email + Background service)
builder.Services.AddSingleton<EmailService>();
builder.Services.AddHostedService<DailyReportService>();

var app = builder.Build();

// 6️⃣ Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ⚠️ Session PRED Authorization
app.UseSession();
app.UseAuthorization();

// 7️⃣ Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

// 8️⃣ Ustvari DB ob zagonu
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();

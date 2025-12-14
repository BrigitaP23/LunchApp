using Microsoft.EntityFrameworkCore;
using LunchApp.Data;
using LunchApp.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// ✅ MVC
builder.Services.AddControllersWithViews();

// ✅ Session (za login)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ Data Protection – shrani ključe za session, da deluje po redeployu
var keysPath = "/app/keys"; // Render / Docker folder
if (!Directory.Exists(keysPath))
{
    Directory.CreateDirectory(keysPath);
}

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
    .SetApplicationName("LunchApp");

// ✅ Render PostgreSQL DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("RenderDb"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);
        }));



// ✅ Servisi
builder.Services.AddSingleton<EmailService>();
builder.Services.AddHostedService<DailyReportService>();

var app = builder.Build();

// ✅ Samodejno posodobi bazo ob zagonu (migrate)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// ✅ Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ⚠️ Session mora biti pred Authorization
app.UseSession();
app.UseAuthorization();

// ✅ Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();

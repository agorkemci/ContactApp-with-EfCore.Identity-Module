using ContactApp.Controllers;
using ContactApp.Models.Identity;
using ContactApp.Repositories;
using ContactApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();//kontroller ve view ekleme

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    // If the connection uses a relative App_Data path, make it absolute so EF uses the same file
    if (!string.IsNullOrEmpty(connectionString) && connectionString.StartsWith("Data Source=App_Data", StringComparison.OrdinalIgnoreCase))
    {
        var relative = connectionString.Substring("Data Source=".Length).Trim();
        var absolutePath = Path.Combine(builder.Environment.ContentRootPath, relative.Replace('/', Path.DirectorySeparatorChar));
        var finalConnectionString = $"Data Source={absolutePath}";
        options.UseSqlite(finalConnectionString);
    }
    else
    {
        options.UseSqlite(connectionString);
    }
});
// ASP.NET Core Identity üyelik ve yetkilendirme sistemini projeye ekliyoruz.
// Kullanıcı profilini 'ApplicationUser', rol yapısını 'ApplicationRole' sınıflarıyla temsil ediyoruz.
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // --- Şifre Güvenlik Kuralları (Geliştirme ortamında rahat test yapmak için gevşetildi) ---
    options.Password.RequiredLength = 6;           // Şifre en az 6 karakter uzunluğunda olmalıdır.
    options.Password.RequireDigit = false;          // Rakam (0-9) içerme zorunluluğunu kaldırır.
    options.Password.RequireLowercase = false;      // Küçük harf (a-z) içerme zorunluluğunu kaldırır.
    options.Password.RequireUppercase = false;      // Büyük harf (A-Z) içerme zorunluluğunu kaldırır.
    options.Password.RequireNonAlphanumeric = false; // Özel karakter (!, @, #, ?, * vb.) zorunluluğunu kaldırır.

    // --- Kullanıcı ve Oturum Açma Kuralları ---
    options.User.RequireUniqueEmail = true;         // Bir e-posta adresiyle sadece tek bir hesap açılabilir (Benzersizlik şartı).
    options.SignIn.RequireConfirmedEmail = false;   // Kullanıcının kayıt sonrası e-posta doğrulama linkine tıklamadan giriş yapabilmesine izin verir.
})
    // Kullanıcıları, şifre özetlerini (hash) ve rolleri ApplicationDbContext üzerinden veritabanı tablolarına yazar/okur.
    .AddEntityFrameworkStores<ApplicationDbContext>()

    // Şifre sıfırlama ve iki adımlı doğrulama (2FA) gibi işlemler için güvenli onay jetonları (token) üreten servisleri ekler.
    .AddDefaultTokenProviders();


// Oturum açma çerezinin (Authentication Cookie) yönlendirme kurallarını yapılandırıyoruz[cite: 2]
builder.Services.ConfigureApplicationCookie(options =>
{
// Giriş yapmamış bir kullanıcı kilitli ([Authorize]) bir sayfaya gitmek isterse buraya yönlendirilir (401 Unauthorized)[cite: 2]
options.LoginPath = "/Account/Login";

    // Giriş yapmış ama rolü/yetkisi o sayfaya yetmeyen bir kullanıcı buraya yönlendirilir (403 Forbidden)[cite: 2]
    options.AccessDeniedPath = "/Account/AccessDenied"; 
}); 

//Dependency Injection - Register the IContactRepository interface with db
builder.Services.AddScoped<IContactRepository, EfContactRepository>();//her istekte tek bir EFcontactRepo nesnesi kullanılacak


builder.Services.AddScoped<IAuthService, AuthManager>();
var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
builder.Services.AddHttpClient<INewsService, NewsService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);//Inewservice çağrıldığında newsservice otomatik olarak dönerken client ifadesi için base url i doğrudan çözümlüyor


});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    ;

// Uygulama ayağa kalkarken Scoped yaşam döngüsüne sahip servisleri (DbContext gibi)
// güvenle tüketebilmek için manuel bir bağımlılık çözme kapsamı (scope) oluşturulur.
// using bloğu sona erdiğinde bu scope ve türetilen nesneler bellekten (dispose) temizlenir.
using (var scope = app.Services.CreateScope())
{
    // 1. Uygulamanın ana dizininde "App_Data" klasörünün tam yolunu oluştur
    var dataDir = Path.Combine(app.Environment.ContentRootPath, "App_Data");

    // Klasör disk üzerinde mevcut değilse oluştur (genellikle SQLite veya yerel dosya tabanlı DB'ler için)
    Directory.CreateDirectory(dataDir);

    // 2. DI konteyneri üzerinden ApplicationDbContext örneğini çöz (resolve et)
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    try
    {
        // If SQLite WAL/SHM files exist from previous runs, remove them to avoid file locks
        var dbFile = Path.Combine(dataDir, "contacts.db");
        var walFile = dbFile + "-wal";
        var shmFile = dbFile + "-shm";
        if (File.Exists(walFile)) File.Delete(walFile);
        if (File.Exists(shmFile)) File.Delete(shmFile);

        // 3. Varsa bekleyen EF Core migrasyonlarını veri tabanına uygula
        db.Database.Migrate();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        
        
        
        
        
        // 4. Başlangıç verilerini (rol, yönetici hesabı, varsayılan kayıtlar vb.) veri tabanına ekle
        DbSeeder.Seed(db,userManager,roleManager);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Database migration/seed failed");
        throw;
    }
}
app.Run();

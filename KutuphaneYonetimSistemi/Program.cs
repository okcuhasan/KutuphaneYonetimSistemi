using KutuphaneYonetimSistemi.Data;
using KutuphaneYonetimSistemi.Data.Context;
using KutuphaneYonetimSistemi.Service.Abstract;
using KutuphaneYonetimSistemi.Service.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("KutuphaneYonetimiConnectionString"));
});


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddScoped<IGenericRepository<Kitap>, GenericRepository<Kitap>>();
builder.Services.AddScoped<IGenericRepository<Yazar>, GenericRepository<Yazar>>();
builder.Services.AddScoped<IGenericRepository<Yayinevi>, GenericRepository<Yayinevi>>();
builder.Services.AddScoped<IGenericRepository<Kategori>, GenericRepository<Kategori>>();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "KutuphaneSistemiCookie";
    // options.LoginPath = "/GirisYap/GirisYap"; // authorize ile korunan sayfalara sisteme giri� yapmadan girilmeye �al���l�r ise y�nlendirilecek kaynakt�r
    // LogoutPath = kullan�c� ��k�� yapt���nda y�nelece�i adres
    // AccessDeniedPath = kullan�c� yetkisi olmayan sayfaya eri�meye �al���r ise y�nelece�i kaynak
    options.SlidingExpiration = true; // kullan�c�n�n belirli bir zaman aral���nda istek yapmad��� senaryoda oturumun sonlanmas� 30 g�n i�in
    options.ExpireTimeSpan = TimeSpan.FromDays(30); // 30 g�n giri� yap�lmaz ise cookie ler silinsin
});


//builder.Services.Configure<IdentityOptions>(options =>
//{
//    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // 5 dakika bekle
//    options.Lockout.MaxFailedAccessAttempts = 5; // 5 defa hatal� giri� yap�l�r ise
//});


var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Admin", "Kullanici" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

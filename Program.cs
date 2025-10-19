using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using FoodDelivery.Data;
using FoodDelivery.Models;
using FoodDelivery.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Налаштування підключення до бази даних
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 2. Налаштування системи автентифікації (Identity)
builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// 3. Налаштування сесії
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 4. Реєстрація сервісів для кошика
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CartService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Налаштування конвеєра обробки HTTP-запитів
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Початкове заповнення даних (Seeding)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    var configuration = services.GetRequiredService<IConfiguration>(); // Отримуємо доступ до конфігурації

    // Створення ролей з appsettings.json
    var roles = configuration.GetSection("InitialSetup:Roles").Get<List<string>>();
    if (roles != null)
    {
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }

    // Створення користувача-адміністратора з appsettings.json
    var adminEmail = configuration["InitialSetup:AdminEmail"];
    var adminPassword = configuration["InitialSetup:AdminPassword"];
    
    if (!string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminPassword) && await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var admin = new User { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        var result = await userManager.CreateAsync(admin, adminPassword);
        if (result.Succeeded)
        {
            // Додаємо користувача до ролі "Admin" (назва ролі теж береться з конфігурації)
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }

    // Створення тестового ресторану, якщо база даних порожня
    if (!db.Restaurants.Any())
    {
        var r = new Restaurant { Name = "Mama Pizza", Address = "Khreshchatyk 1", Phone = "+380000000000" };
        db.Restaurants.Add(r);
        db.MenuItems.Add(new MenuItem { Restaurant = r, Name = "Pepperoni", Price = 9.99m });
        db.MenuItems.Add(new MenuItem { Restaurant = r, Name = "Margherita", Price = 8.50m });
        await db.SaveChangesAsync();
    }
}

app.Run();
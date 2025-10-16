using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using FoodDelivery.Data;
using FoodDelivery.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Налаштування підключення до бази даних
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 2. Налаштування системи автентифікації (Identity)
// Додає сервіси для користувачів (User) та ролей (IdentityRole)
builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();
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

// 3. Увімкнення автентифікації та авторизації
app.UseAuthentication();
app.UseAuthorization();


// 4. Налаштування маршрутизації
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages(); // Для сторінок Identity (логін, реєстрація)


// 5. Початкове заповнення даних (Seeding)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Застосування міграцій до бази даних
    db.Database.Migrate();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    // Створення ролей, якщо вони не існують
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }
    if (!await roleManager.RoleExistsAsync("RestaurantOwner"))
    {
        await roleManager.CreateAsync(new IdentityRole("RestaurantOwner"));
    }
    // ... і так далі для інших ролей ...

    // Створення користувача-адміністратора, якщо його немає
    if (await userManager.FindByNameAsync("admin") == null)
    {
        var admin = new User { UserName = "admin", Email = "admin@example.com", EmailConfirmed = true };
        var result = await userManager.CreateAsync(admin, "Admin123!");
        if (result.Succeeded)
        {
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
        db.SaveChanges();
    }
}

app.Run();
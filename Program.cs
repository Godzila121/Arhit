using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();


// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
app.UseExceptionHandler("/Home/Error");
app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();
app.UseAuthorization();


app.MapControllerRoute(
name: "areas",
pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
name: "default",
pattern: "{controller=Home}/{action=Index}/{id?}");


// Seed data (простий)
using (var scope = app.Services.CreateScope())
{
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
db.Database.EnsureCreated();
if(!db.Restaurants.Any())
{
var r = new Restaurant { Name = "Mama Pizza", Address = "Khreshchatyk 1", Phone = "+380000000000" };
db.Restaurants.Add(r);
db.MenuItems.Add(new MenuItem { Restaurant = r, Name = "Pepperoni", Price = 9.99m });
db.MenuItems.Add(new MenuItem { Restaurant = r, Name = "Margherita", Price = 8.50m });
db.SaveChanges();
}
}


app.Run();
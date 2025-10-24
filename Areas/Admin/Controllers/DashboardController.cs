// Areas/Admin/Controllers/DashboardController.cs
using FoodDelivery.Data;
using FoodDelivery.Models; // <-- Додайте цей using
using FoodDelivery.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; // <-- Додайте цей using
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // <-- Додайте цей using
using System.Linq;
using System.Threading.Tasks; // <-- Додайте цей using

namespace FoodDelivery.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<User> _userManager; // <-- Поле для UserManager

        // Оновлений конструктор для отримання обох сервісів
        public DashboardController(AppDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index() // <-- Змінено на async Task
        {
            // Отримуємо дані для панелі
            var totalOrders = await _db.Orders.CountAsync();
            var totalRestaurants = await _db.Restaurants.CountAsync();
            var averageRating = await _db.Reviews.AnyAsync() ? await _db.Reviews.AverageAsync(r => r.Rating) : 0;
            var totalUsers = await _db.Users.CountAsync(); // Або _userManager.Users.CountAsync();
            var totalCouriers = await _db.Couriers.CountAsync();
            var recentOrders = await _db.Orders
                                        .Include(o => o.User) // Завантажуємо дані про користувача
                                        .OrderByDescending(o => o.CreatedAt)
                                        .Take(10)
                                        .ToListAsync();

            // Створюємо та заповнюємо ViewModel
            var viewModel = new AdminDashboardViewModel
            {
                TotalOrders = totalOrders,
                TotalRestaurants = totalRestaurants,
                AverageRating = averageRating,
                TotalUsers = totalUsers,
                TotalCouriers = totalCouriers,
                RecentOrders = recentOrders
            };

            // Передаємо ViewModel у представлення
            return View(viewModel);
        }
    }
}
// Areas/Admin/Controllers/DashboardController.cs
using FoodDelivery.Data;
using FoodDelivery.ViewModels; // <-- ДОДАЙТЕ ЦЕЙ using
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // <-- ВИПРАВЛЕНО: Тільки "Admin"
    public class DashboardController : Controller
    {
        private readonly AppDbContext _db;
        public DashboardController(AppDbContext db) { _db = db; }

        public IActionResult Index()
        {
            // Отримуємо дані для панелі
            var totalOrders = _db.Orders.Count();
            var totalRestaurants = _db.Restaurants.Count();
            var averageRating = _db.Reviews.Any() ? _db.Reviews.Average(r => r.Rating) : 0;

            // Створюємо та заповнюємо ViewModel
            var viewModel = new AdminDashboardViewModel
            {
                TotalOrders = totalOrders,
                TotalRestaurants = totalRestaurants,
                AverageRating = averageRating
            };

            // Передаємо ViewModel у представлення
            return View(viewModel); // <-- ОНОВЛЕНО: Повертаємо ViewModel
        }
    }
}
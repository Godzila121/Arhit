// Areas/Restaurant/Controllers/DashboardController.cs
using FoodDelivery.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FoodDelivery.Areas.Restaurant.Controllers // <--- ДОДАНО: Простір імен
{
    [Area("Restaurant")] // <--- ДОДАНО: Атрибут Area для цієї області
    [Authorize] // Доступ тільки для авторизованих
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Отримуємо ID поточного користувача
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Знаходимо ресторан, який належить цьому користувачу
            var restaurant = await _context.Restaurants
                .Include(r => r.MenuItems) // Підвантажуємо меню
                .Include(r => r.Orders)    // Підвантажуємо замовлення
                .FirstOrDefaultAsync(r => r.OwnerId == userId);

            // Якщо у користувача ще немає ресторану
            if (restaurant == null)
            {
                // TODO: Перенаправити на сторінку створення ресторану або показати повідомлення
                return View("NoRestaurant"); 
            }

            return View(restaurant);
        }
    }
} // <--- ДОДАНО: Закриваюча дужка для простору імен
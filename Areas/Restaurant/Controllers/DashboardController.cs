using FoodDelivery.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FoodDelivery.Areas.Restaurant.Controllers
{
    [Area("Restaurant")]
    // ОНОВЛЕНО: Доступ тільки для власників ресторанів та адміністраторів
    [Authorize(Roles = "RestaurantOwner, Admin")] 
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
                // Якщо користувач - адмін, він може не мати ресторану.
                // Можна додати логіку для адміна або просто показати, що ресторану немає.
                if (User.IsInRole("Admin"))
                {
                    // Можливо, перенаправити адміна на іншу сторінку
                    // або показати повідомлення "Ви увійшли як адмін".
                }
                
                return View("NoRestaurant"); 
            }

            return View(restaurant);
        }
    }
}

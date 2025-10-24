using FoodDelivery.Data;
using FoodDelivery.Models; // <-- Переконайтеся, що цей using є
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FoodDelivery.Areas.RestaurantOwner.Controllers
{
    [Area("RestaurantOwner")]
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // --- ОНОВЛЕНИЙ ЗАПИТ ДО БАЗИ ДАНИХ ---
            var restaurant = await _context.Restaurants
                .Include(r => r.MenuItems)
                .Include(r => r.Orders)
                    .ThenInclude(o => o.User) // <-- ОСНОВНЕ ВИПРАВЛЕННЯ: завантажуємо користувача для кожного замовлення
                .FirstOrDefaultAsync(r => r.OwnerId == userId);

            if (restaurant == null)
            {
                if (User.IsInRole("Admin"))
                {
                    // Для адміна це нормально, показуємо йому сторінку-заглушку
                    return View("NoRestaurant");
                }
                
                // Для власника ресторану без ресторану
                return View("NoRestaurant"); 
            }

            return View(restaurant);
        }
    }
}
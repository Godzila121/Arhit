using FoodDelivery.Data;
using FoodDelivery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // <-- Переконайтеся, що цей using є
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FoodDelivery.Areas.RestaurantOwner.Controllers
{
    [Area("RestaurantOwner")]
    [Authorize(Roles = "RestaurantOwner,Admin")]
    public class MenuController : Controller
    {
        private readonly AppDbContext _context;

        public MenuController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.OwnerId == userId);

            if (restaurant == null)
            {
                // Якщо у користувача немає ресторану, можливо, перенаправити
                // його на сторінку створення або показати повідомлення.
                return View("NoRestaurant"); // Або інша логіка
            }

            // --- ОНОВЛЕНИЙ ЗАПИТ ---
            var menuItems = await _context.MenuItems
                .Where(m => m.RestaurantId == restaurant.Id)
                .Include(m => m.Restaurant) // <-- ОСЬ ВИРІШЕННЯ: додаємо дані про ресторан
                .ToListAsync();

            return View(menuItems);
        }
        // GET: Restaurant/Menu/Create
// Цей метод просто показує сторінку з формою для створення страви.
public IActionResult Create()
{
    return View();
}

// POST: Restaurant/Menu/Create
// Цей метод отримує дані, надіслані з форми.
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([Bind("Name,Description,Price")] MenuItem menuItem)
{
    // Знаходимо ресторан поточного користувача, щоб прив'язати до нього нову страву.
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.OwnerId == userId);

    if (restaurant == null)
    {
        // Якщо раптом ресторан не знайдено, повертаємо помилку.
        return NotFound("Ваш ресторан не знайдено.");
    }

    // Перевіряємо, чи всі дані з форми коректні (наприклад, чи заповнені обов'язкові поля).
    if (ModelState.IsValid)
    {
        // Призначаємо ID ресторану новій страві.
        menuItem.RestaurantId = restaurant.Id;

        // Додаємо новий об'єкт страви до контексту бази даних.
        _context.Add(menuItem);

        // Зберігаємо зміни в базі даних.
        await _context.SaveChangesAsync();

        // Перенаправляємо користувача назад на сторінку з повним списком меню.
        return RedirectToAction(nameof(Index));
    }

    // Якщо дані в формі були некоректні, показуємо форму знову, але вже з повідомленнями про помилки.
    return View(menuItem);
}
    }
}
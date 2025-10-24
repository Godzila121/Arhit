using FoodDelivery.Data;
using FoodDelivery.Models; // <-- Додаємо using для моделі User
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; // <-- Додаємо using для UserManager
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FoodDelivery.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RestaurantController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager; // <-- 1. Поле для UserManager

        // 2. Оновлюємо конструктор, щоб отримати UserManager
        public RestaurantController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 3. Покращуємо метод Index
        public async Task<IActionResult> Index()
        {
            // Додаємо .Include(r => r.Owner), щоб разом з ресторанами
            // завантажити інформацію про їхніх власників.
            var restaurants = await _context.Restaurants.Include(r => r.Owner).ToListAsync();
            return View(restaurants);
        }
        // GET: Admin/Restaurant/Create
        // Цей метод відповідає за показ сторінки з формою для створення ресторану.
        public async Task<IActionResult> Create()
        {
            // 1. Отримуємо всіх користувачів, які мають роль "RestaurantOwner".
            var owners = await _userManager.GetUsersInRoleAsync("RestaurantOwner");

            // 2. Створюємо випадаючий список з цих користувачів і передаємо його на сторінку.
            //    Це дозволить вибрати власника зі списку, а не вводити його ID вручну.
            ViewData["OwnerId"] = new SelectList(owners, "Id", "UserName");

            return View();
        }
        // GET: Admin/Restaurant/Details/5
// Показує детальну інформацію про обраний ресторан.
public async Task<IActionResult> Details(int? id)
{
    if (id == null) return NotFound();
    
    var restaurant = await _context.Restaurants
        .Include(r => r.Owner) // Підвантажуємо дані про власника
        .Include(r => r.MenuItems) // Підвантажуємо дані про меню
        .FirstOrDefaultAsync(m => m.Id == id);
        
    if (restaurant == null) return NotFound();

    return View(restaurant);
}

// GET: Admin/Restaurant/Edit/5
// Показує форму для редагування існуючого ресторану.
public async Task<IActionResult> Edit(int? id)
{
    if (id == null) return NotFound();

    var restaurant = await _context.Restaurants.FindAsync(id);
    if (restaurant == null) return NotFound();

    // Знову завантажуємо список власників для випадаючого списку.
    var owners = await _userManager.GetUsersInRoleAsync("RestaurantOwner");
    ViewData["OwnerId"] = new SelectList(owners, "Id", "UserName", restaurant.OwnerId);
    return View(restaurant);
}

// POST: Admin/Restaurant/Edit/5
// Обробляє дані з форми редагування.
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Phone,Address,OwnerId")] Restaurant restaurant)
{
    if (id != restaurant.Id) return NotFound();

    if (ModelState.IsValid)
    {
        try
        {
            _context.Update(restaurant);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Restaurants.Any(e => e.Id == restaurant.Id)) return NotFound();
            else throw;
        }
        return RedirectToAction(nameof(Index));
    }
    
    // Якщо дані некоректні, повертаємо форму з помилками.
    var owners = await _userManager.GetUsersInRoleAsync("RestaurantOwner");
    ViewData["OwnerId"] = new SelectList(owners, "Id", "UserName", restaurant.OwnerId);
    return View(restaurant);
}

// GET: Admin/Restaurant/Delete/5
// Показує сторінку з підтвердженням видалення.
public async Task<IActionResult> Delete(int? id)
{
    if (id == null) return NotFound();

    var restaurant = await _context.Restaurants
        .Include(r => r.Owner)
        .FirstOrDefaultAsync(m => m.Id == id);
    if (restaurant == null) return NotFound();

    return View(restaurant);
}

// POST: Admin/Restaurant/Delete/5
// Виконує фактичне видалення ресторану.
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    var restaurant = await _context.Restaurants.FindAsync(id);
    if (restaurant != null)
    {
        _context.Restaurants.Remove(restaurant);
    }

    await _context.SaveChangesAsync();
    return RedirectToAction(nameof(Index));
}

        // POST: Admin/Restaurant/Create
        // Цей метод спрацьовує, коли ви заповнюєте форму і натискаєте кнопку "Створити".
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Phone,Address,OwnerId")] Restaurant restaurant)
        {
            // Перевіряємо, чи всі дані, що прийшли з форми, є коректними.
            if (ModelState.IsValid)
            {
                // Якщо все добре, додаємо новий ресторан в базу даних.
                _context.Add(restaurant);
                await _context.SaveChangesAsync();

                // Після успішного збереження перенаправляємо на головну сторінку управління ресторанами.
                return RedirectToAction(nameof(Index));
            }

            // Якщо дані були некоректними, нам потрібно знову завантажити список власників
            // і показати форму ще раз, але вже з повідомленнями про помилки.
            var owners = await _userManager.GetUsersInRoleAsync("RestaurantOwner");
            ViewData["OwnerId"] = new SelectList(owners, "Id", "UserName", restaurant.OwnerId);

            return View(restaurant);
        }
    }
    
}
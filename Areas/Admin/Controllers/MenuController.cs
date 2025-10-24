using FoodDelivery.Data;
using FoodDelivery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MenuController : Controller
    {
        private readonly AppDbContext _context;

        public MenuController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Menu/Index/5 (Список страв ресторану)
        public async Task<IActionResult> Index(int restaurantId)
        {
            var restaurant = await _context.Restaurants.FindAsync(restaurantId);
            if (restaurant == null) return NotFound();

            var menuItems = await _context.MenuItems
                .Where(m => m.RestaurantId == restaurantId)
                .ToListAsync();

            ViewBag.RestaurantId = restaurant.Id;
            ViewBag.RestaurantName = restaurant.Name;

            return View(menuItems);
        }

        // --- ДОДАЄМО НОВІ МЕТОДИ ---

        // GET: Admin/Menu/Create?restaurantId=5 (Показати форму створення)
        public IActionResult Create(int restaurantId)
        {
            // Передаємо ID ресторану, щоб знати, до якого ресторану додавати страву
            ViewBag.RestaurantId = restaurantId;
            return View();
        }

        // POST: Admin/Menu/Create (Обробити форму створення)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int restaurantId, [Bind("Name,Description,Price")] MenuItem menuItem)
        {
            if (ModelState.IsValid)
            {
                menuItem.RestaurantId = restaurantId; // Прив'язуємо страву до ресторану
                _context.Add(menuItem);
                await _context.SaveChangesAsync();
                // Повертаємось до списку меню саме цього ресторану
                return RedirectToAction(nameof(Index), new { restaurantId = restaurantId });
            }
            // Якщо помилка, повертаємо на форму, не забувши передати ID ресторану
            ViewBag.RestaurantId = restaurantId;
            return View(menuItem);
        }

        // GET: Admin/Menu/Edit/15 (Показати форму редагування страви з ID=15)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null) return NotFound();

            // Зберігаємо ID ресторану для повернення на правильну сторінку
            ViewBag.RestaurantId = menuItem.RestaurantId; 
            return View(menuItem);
        }

        // POST: Admin/Menu/Edit/15 (Обробити форму редагування)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,RestaurantId")] MenuItem menuItem)
        {
            if (id != menuItem.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(menuItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.MenuItems.Any(e => e.Id == menuItem.Id)) return NotFound();
                    else throw;
                }
                // Повертаємось до списку меню ресторану, якому належить ця страва
                return RedirectToAction(nameof(Index), new { restaurantId = menuItem.RestaurantId });
            }
            ViewBag.RestaurantId = menuItem.RestaurantId; 
            return View(menuItem);
        }

        // GET: Admin/Menu/Delete/15 (Показати підтвердження видалення)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menuItem == null) return NotFound();

            ViewBag.RestaurantId = menuItem.RestaurantId; 
            return View(menuItem);
        }

        // POST: Admin/Menu/Delete/15 (Видалити страву)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            int restaurantId = 0; // Змінна для збереження ID ресторану
            if (menuItem != null)
            {
                restaurantId = menuItem.RestaurantId; // Запам'ятовуємо ID перед видаленням
                _context.MenuItems.Remove(menuItem);
                await _context.SaveChangesAsync();
            }

            // Повертаємось до списку меню ресторану
            return RedirectToAction(nameof(Index), new { restaurantId = restaurantId });
        }
    }
}
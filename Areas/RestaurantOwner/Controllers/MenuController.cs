using FoodDelivery.Data;
using FoodDelivery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FoodDelivery.Areas.RestaurantOwner.Controllers
{
    [Area("RestaurantOwner")]
    [Authorize(Roles = "RestaurantOwner, Admin")]
    public class MenuController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MenuController> _logger; 

        public MenuController(AppDbContext context, ILogger<MenuController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? restaurantId)
        {
            if (restaurantId == null)
            {
                ViewBag.Restaurants = new SelectList(await _context.Restaurants.ToListAsync(), "Id", "Name");
                return View("SelectRestaurant");
            }

            var restaurant = await _context.Restaurants.FindAsync(restaurantId.Value);
            if (restaurant == null)
            {
                return NotFound("Ресторан не знайдено.");
            }

            ViewBag.RestaurantId = restaurant.Id;
            ViewBag.RestaurantName = restaurant.Name;

            var menuItems = await _context.MenuItems
                .Where(m => m.RestaurantId == restaurant.Id)
                .ToListAsync();

            return View(menuItems);
        }

        public async Task<IActionResult> Create(int restaurantId)
        {
            var restaurant = await _context.Restaurants.FindAsync(restaurantId);
            if (restaurant == null)
            {
                return NotFound("Ресторан для додавання страви не знайдено.");
            }
            ViewBag.RestaurantId = restaurantId;
            ViewBag.RestaurantName = restaurant.Name;
            return View();
        }

 

[HttpPost]
[ValidateAntiForgeryToken]

public async Task<IActionResult> Create([Bind("Name,Description,Price,RestaurantId")] MenuItem menuItem)
{
    if (menuItem.RestaurantId == 0)
    {
        ModelState.AddModelError("RestaurantId", "Необхідно вказати ресторан.");
    }
    else
    {
        var restaurantExists = await _context.Restaurants.AnyAsync(r => r.Id == menuItem.RestaurantId);
        if (!restaurantExists)
        {
            ModelState.AddModelError("RestaurantId", "Обраний ресторан не знайдено.");
        }
    }

    ModelState.Remove("Restaurant");

    if (!ModelState.IsValid)
    {
        _logger.LogWarning("ModelState НЕ валідний. Помилки:");
        foreach (var state in ModelState)
        {
            foreach (var error in state.Value.Errors)
            {
                _logger.LogWarning("- Поле: {Field}, Помилка: {ErrorMessage}", state.Key, error.ErrorMessage);
            }
        }
        var errorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        ViewBag.ValidationErrors = errorMessages;
    }
    else
    {
         _logger.LogInformation("ModelState валідний.");
    }

    if (ModelState.IsValid)
    {
        _logger.LogInformation("Спроба додати MenuItem (через модель): Name={Name}, Price={Price}, RestaurantId={RestaurantId}",
               menuItem.Name, menuItem.Price, menuItem.RestaurantId);

       try
            {
                _context.Add(menuItem);
                var entry = _context.Entry(menuItem);
                 _logger.LogInformation("Стан MenuItem ПІСЛЯ Add: {EntityState}", entry.State);

                int result = await _context.SaveChangesAsync();
                _logger.LogInformation("SaveChangesAsync завершено. Результат: {Result}", result);

                if (result > 0)
                {
                    _logger.LogInformation("Успішно збережено MenuItem з ID: {MenuItemId}", menuItem.Id);
                    return RedirectToAction(nameof(Index), new { restaurantId = menuItem.RestaurantId });
                }
                else
                {
                    _logger.LogWarning("SaveChangesAsync повернув 0, хоча ModelState був валідний.");
                    ModelState.AddModelError("", "Не вдалося зберегти страву (SaveChanges = 0).");
                }
            }
        catch (Exception ex)
        {
            _logger.LogError(ex, "КРИТИЧНА ПОМИЛКА під час SaveChangesAsync для MenuItem (з моделі).");
            ModelState.AddModelError("", "Сталася критична помилка під час збереження.");
        }
    }

    _logger.LogWarning("Повернення View Create через невалідний ModelState або помилку збереження.");
    if (menuItem.RestaurantId != 0)
    {
        var restaurant = await _context.Restaurants.FindAsync(menuItem.RestaurantId);
        if (restaurant != null)
        {
            ViewBag.RestaurantId = restaurant.Id;
            ViewBag.RestaurantName = restaurant.Name;
        }
        else
        {
            ViewBag.RestaurantName = "Невідомий ресторан";
        }
    }

    return View(menuItem);
}


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var menuItem = await _context.MenuItems.Include(mi => mi.Restaurant).FirstOrDefaultAsync(mi => mi.Id == id);
            if (menuItem == null) return NotFound();
            return View(menuItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,RestaurantId")] MenuItem menuItem)
        {
             if (id != menuItem.Id) return NotFound();

             // Перевірка, чи ресторан існує (якщо раптом його видалили)
             var restaurantExists = await _context.Restaurants.AnyAsync(r => r.Id == menuItem.RestaurantId);
             if (!restaurantExists)
             {
                 ModelState.AddModelError("RestaurantId", "Ресторан не знайдено.");
             }
             
             // --- 
             // --- 🚀 ДОДАЙТЕ ЦЕ ТАКОЖ І СЮДИ (для редагування) 🚀 ---
             ModelState.Remove("Restaurant");
             // --- 

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
                 return RedirectToAction(nameof(Index), new { restaurantId = menuItem.RestaurantId });
             }
             // Якщо не валідно, підвантажуємо назву ресторану для View
              var restaurant = await _context.Restaurants.FindAsync(menuItem.RestaurantId);
              if (restaurant != null) ViewBag.RestaurantName = restaurant.Name; // Для відображення у View

             return View(menuItem);
        }


         // GET: RestaurantOwner/Menu/Delete/5
         public async Task<IActionResult> Delete(int? id)
         {
             if (id == null) return NotFound();
             var menuItem = await _context.MenuItems
                 .Include(m => m.Restaurant) // Включаємо ресторан для відображення назви
                 .FirstOrDefaultAsync(m => m.Id == id);
             if (menuItem == null) return NotFound();
             return View(menuItem);
         }

        // POST: RestaurantOwner/Menu/Delete/5
         [HttpPost, ActionName("Delete")]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> DeleteConfirmed(int id)
         {
             var menuItem = await _context.MenuItems.FindAsync(id);
             int? restaurantId = menuItem?.RestaurantId; // Зберігаємо ID ресторану перед видаленням
             if (menuItem != null)
             {
                 _context.MenuItems.Remove(menuItem);
                 await _context.SaveChangesAsync();
             }
             // Перенаправляємо на меню того ж ресторану (якщо вдалося отримати ID)
             return RedirectToAction(nameof(Index), new { restaurantId = restaurantId });
         }

    }
}
using FoodDelivery.Data;
using FoodDelivery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // <-- Додай це для SelectList
using Microsoft.EntityFrameworkCore;
using System.Linq;
// using System.Security.Claims; // Можна видалити, якщо OwnerId більше не використовується
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // <-- ДОДАЙ ЦЕЙ USING

namespace FoodDelivery.Areas.RestaurantOwner.Controllers
{
    [Area("RestaurantOwner")] // Назву зони можна залишити, або перенести контролер в Admin зону
    [Authorize(Roles = "Admin")]
    public class MenuController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MenuController> _logger; // <-- ДОДАНО ПОЛЕ

        public MenuController(AppDbContext context)
        {
            _context = context;
        }

        // GET: RestaurantOwner/Menu/Index/{restaurantId?}
        // Приймає ID ресторану як параметр маршруту
        public async Task<IActionResult> Index(int? restaurantId)
        {
            if (restaurantId == null)
            {
                // Якщо ID не передано, можливо, показати список ресторанів для вибору
                // Або перенаправити на сторінку вибору ресторану
                ViewBag.Restaurants = new SelectList(await _context.Restaurants.ToListAsync(), "Id", "Name");
                return View("SelectRestaurant"); // Потрібно створити View SelectRestaurant.cshtml
            }

            var restaurant = await _context.Restaurants.FindAsync(restaurantId.Value);
            if (restaurant == null)
            {
                return NotFound("Ресторан не знайдено.");
            }

            // Зберігаємо ID і назву ресторану для використання у View (напр., для кнопки "Створити")
            ViewBag.RestaurantId = restaurant.Id;
            ViewBag.RestaurantName = restaurant.Name;

            var menuItems = await _context.MenuItems
                .Where(m => m.RestaurantId == restaurant.Id)
                // .Include(m => m.Restaurant) // Включати ресторан вже не обов'язково, бо ми його знайшли
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
            // Передаємо ID ресторану у View, щоб форма знала, куди відправляти дані
            ViewBag.RestaurantId = restaurantId;
            ViewBag.RestaurantName = restaurant.Name; // Для відображення назви ресторану
            return View();
        }

 

[HttpPost]
[ValidateAntiForgeryToken]

public async Task<IActionResult> Create([Bind("Name,Description,Price,RestaurantId")] MenuItem menuItem)
{
    // 1. Додаткова перевірка існування ресторану (можна залишити)
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

    // --- ДОДАЄМО ДІАГНОСТИКУ МОДЕЛІ ---
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
        // Збираємо помилки для ViewBag, щоб спробувати показати їх у View
        var errorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        ViewBag.ValidationErrors = errorMessages;
    }
    else
    {
         _logger.LogInformation("ModelState валідний.");
    }
    // --- КІНЕЦЬ ДІАГНОСТИКИ ---

    // 2. Перевіряємо ModelState (включає атрибути моделі [Required], [Range] та помилки, додані вище)
    if (ModelState.IsValid)
    {
        _logger.LogInformation("Спроба додати MenuItem (через модель): Name={Name}, Price={Price}, RestaurantId={RestaurantId}",
               menuItem.Name, menuItem.Price, menuItem.RestaurantId);

       try
            {
                // *** ЗМІНА ТУТ: Явно встановлюємо стан ***
                _context.Entry(menuItem).State = EntityState.Added;
                // _context.Add(menuItem); // Цей рядок тепер може бути непотрібним, але можна залишити

                var entry = _context.Entry(menuItem);
                _logger.LogInformation("Стан MenuItem ПЕРЕД SaveChanges (після явного встановлення): {EntityState}", entry.State); // Логуємо стан

                int result = await _context.SaveChangesAsync();
                _logger.LogInformation("SaveChangesAsync завершено. Результат: {Result}", result);

                if (result > 0)
                {
                    _logger.LogInformation("Успішно збережено MenuItem з ID: {MenuItemId}", menuItem.Id);
                    return RedirectToAction(nameof(Index), new { restaurantId = menuItem.RestaurantId });
                }
                else
                {
                    _logger.LogWarning("SaveChangesAsync повернув 0, навіть після явного встановлення стану Added.");
                    ModelState.AddModelError("", "Не вдалося зберегти страву (SaveChanges = 0).");
                }
            }
        catch (Exception ex)
        {
            _logger.LogError(ex, "КРИТИЧНА ПОМИЛКА під час SaveChangesAsync для MenuItem (з моделі).");
            ModelState.AddModelError("", "Сталася критична помилка під час збереження.");
        }
    }

    // 3. Якщо ModelState НЕ валідний АБО збереження не вдалося
    _logger.LogWarning("Повернення View Create через невалідний ModelState або помилку збереження.");
    // Потрібно знову отримати назву ресторану для ViewBag
    if (menuItem.RestaurantId != 0)
    {
        var restaurant = await _context.Restaurants.FindAsync(menuItem.RestaurantId);
        if (restaurant != null)
        {
            ViewBag.RestaurantId = restaurant.Id; // Передаємо ID назад у View
            ViewBag.RestaurantName = restaurant.Name;
        }
        else
        {
            ViewBag.RestaurantName = "Невідомий ресторан"; // Якщо раптом ресторан видалили
        }
    }

    // Повертаємо ту саму View з помилками валідації
    return View(menuItem); // Повертаємо модель з даними, які ввів користувач
}


        // GET: RestaurantOwner/Menu/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var menuItem = await _context.MenuItems.Include(mi => mi.Restaurant).FirstOrDefaultAsync(mi => mi.Id == id);
            if (menuItem == null) return NotFound();
            // Можливо, тут не потрібен SelectList, якщо ресторан не змінюється
            return View(menuItem);
        }

        // POST: RestaurantOwner/Menu/Edit/5
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
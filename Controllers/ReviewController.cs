// Controllers/ReviewController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FoodDelivery.Models; // Щоб тільки авторизовані користувачі залишали відгуки

[Authorize]
public class ReviewController : Controller
{
    // Логіка для створення відгуку
    [HttpPost]
    public IActionResult Create(Review review)
    {
        if (ModelState.IsValid)
        {
            // Збереження відгуку в базу даних
            return RedirectToAction("Details", "Restaurant", new { id = review.RestaurantId });
        }
        // Повернення на сторінку з помилкою
        return View(review);
    }
}
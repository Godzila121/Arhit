using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FoodDelivery.Models;

[Authorize]
public class ReviewController : Controller
{
    [HttpPost]
    public IActionResult Create(Review review)
    {
        if (ModelState.IsValid)
        {
            return RedirectToAction("Details", "Restaurant", new { id = review.RestaurantId });
        }
        return View(review);
    }
}
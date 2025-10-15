using FoodDelivery.Data;
using FoodDelivery.Models;
using Microsoft.AspNetCore.Mvc;
[Area("Restaurant")]
public class MenuController : Controller
{
private readonly AppDbContext _db;
public MenuController(AppDbContext db) { _db = db; }


public IActionResult Index(int restaurantId)
{
var items = _db.MenuItems.Where(mi => mi.RestaurantId == restaurantId).ToList();
ViewBag.RestaurantId = restaurantId;
return View(items);
}


[HttpGet]
public IActionResult Create(int restaurantId)
{
ViewBag.RestaurantId = restaurantId;
return View();
}


[HttpPost]
public IActionResult Create(MenuItem model)
{
if (!ModelState.IsValid) return View(model);
_db.MenuItems.Add(model);
_db.SaveChanges();
return RedirectToAction("Index", new { restaurantId = model.RestaurantId });
}
}
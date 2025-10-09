using Microsoft.AspNetCore.Mvc;
[Area("Admin")]
public class DashboardController : Controller
{
private readonly AppDbContext _db;
public DashboardController(AppDbContext db) { _db = db; }


public IActionResult Index()
{
var totalOrders = _db.Orders.Count();
var totalRestaurants = _db.Restaurants.Count();
var averageRating = _db.Reviews.Any() ? _db.Reviews.Average(r => r.Rating) : 0;
var vm = new { totalOrders, totalRestaurants, averageRating };
return View(vm);
}
}
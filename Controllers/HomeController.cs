using Microsoft.AspNetCore.Mvc;


public class HomeController : Controller
{
private readonly AppDbContext _db;
public HomeController(AppDbContext db) { _db = db; }


public IActionResult Index()
{
var restaurants = _db.Restaurants.Take(10).ToList();
return View(restaurants);
}
}
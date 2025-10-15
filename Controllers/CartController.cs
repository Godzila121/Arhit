// Controllers/CartController.cs
using FoodDelivery.Data;
using Microsoft.AspNetCore.Mvc;
// + інші необхідні using

public class CartController : Controller
{
    private readonly AppDbContext _context;

    public CartController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        // Логіка для отримання даних кошика з сесії
        return View();
    }

    public IActionResult AddToCart(int menuItemId)
    {
        // Логіка для додавання товару в сесію
        return RedirectToAction("Index", "Menu", new { restaurantId = GetRestaurantId(menuItemId) });
    }

    // Допоміжний метод, щоб повернутись у меню того ж ресторану
    private int GetRestaurantId(int menuItemId)
    {
        var menuItem = _context.MenuItems.Find(menuItemId);
        return menuItem?.RestaurantId ?? 0;
    }
}
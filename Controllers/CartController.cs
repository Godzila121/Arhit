using FoodDelivery.Data;
using FoodDelivery.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class CartController : Controller
{
    private readonly AppDbContext _context;
    private readonly CartService _cartService;

    public CartController(AppDbContext context, CartService cartService)
    {
        _context = context;
        _cartService = cartService;
    }

    // ЦЕЙ МЕТОД - КЛЮЧ ДО ВИРІШЕННЯ ПРОБЛЕМИ
    // Він має отримувати модель кошика і передавати її у представлення.
    public IActionResult Index()
    {
        var cart = _cartService.GetCart(); // Отримуємо правильну модель
        return View(cart); // Передаємо її у View
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int menuItemId)
    {
        var menuItem = await _context.MenuItems
                                     .Include(mi => mi.Restaurant)
                                     .FirstOrDefaultAsync(mi => mi.Id == menuItemId);

        if (menuItem == null)
        {
            return NotFound();
        }

        _cartService.AddItem(menuItem);

        return RedirectToAction("Details", "Restaurant", new { id = menuItem.RestaurantId });
    }
}

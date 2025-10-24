using FoodDelivery.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

// Додаємо правильний namespace
namespace FoodDelivery.Controllers 
{
    public class CartController : Controller
    {
        // 1. ЗАЛИШАЄМО ТІЛЬКИ СЕРВІС
        // Контролер не повинен знати про базу даних. Він спілкується тільки з сервісом.
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        // 2. ОНОВЛЮЄМО МЕТОД ДЛЯ ВІДОБРАЖЕННЯ КОШИКА
        // Він має повертати не просто список товарів, а ViewModel, 
        // в якій є і товари, і загальна сума.
        public IActionResult Index()
        {
            var cartViewModel = _cartService.GetCartViewModel();
            return View(cartViewModel);
        }

        // 3. ПОВНІСТЮ ВИПРАВЛЯЄМО МЕТОД ДОДАВАННЯ В КОШИК
        public async Task<IActionResult> AddToCart(int menuItemId)
        {
            // Більше ніякої логіки. Просто просимо сервіс додати товар за його ID.
            await _cartService.AddItemToCartAsync(menuItemId);

            // 4. ПОВЕРТАЄМО КОРИСТУВАЧА НАЗАД
            // Цей код поверне користувача на ту сторінку, де він натиснув кнопку "Додати".
            return Redirect(Request.Headers["Referer"].ToString() ?? "/");
        }

        // 5. ДОДАЄМО МЕТОД ДЛЯ ВИДАЛЕННЯ З КОШИКА (знадобиться пізніше)
        public IActionResult RemoveFromCart(int menuItemId)
        {
            _cartService.RemoveItemFromCart(menuItemId);
            return RedirectToAction("Index");
        }
    }
}
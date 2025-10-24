using FoodDelivery.Data;
using FoodDelivery.Models;
using FoodDelivery.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FoodDelivery.Controllers
{
    [Authorize] // Оформлювати замовлення можуть тільки авторизовані користувачі
    public class OrderController : Controller
    {
        private readonly CartService _cartService;
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public OrderController(CartService cartService, AppDbContext context, UserManager<User> userManager)
        {
            _cartService = cartService;
            _context = context;
            _userManager = userManager;
        }

        // GET: /Order/Checkout
        // Показує сторінку з формою для оформлення
        public IActionResult Checkout()
        {
            var cart = _cartService.GetCartViewModel();
            if (!cart.Items.Any())
            {
                // Не дозволяємо оформлювати порожній кошик
                return RedirectToAction("Index", "Cart");
            }
            return View(cart);
        }

        // POST: /Order/Checkout
        // Обробляє дані з форми і створює замовлення
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrder(string deliveryAddress) // Приймаємо адресу з форми
        {
            var cart = _cartService.GetCartViewModel();
            if (!cart.Items.Any())
            {
                ModelState.AddModelError("", "Ваш кошик порожній!");
                return View("Checkout", cart);
            }

            if (string.IsNullOrWhiteSpace(deliveryAddress))
            {
                ModelState.AddModelError("deliveryAddress", "Адреса доставки є обов'язковою.");
                return View("Checkout", cart);
            }
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                Total = cart.Total
                // Можна додати поле Address в модель Order і зберігати його тут
            };

            foreach (var item in cart.Items)
            {
                var orderItem = new OrderItem
                {
                    MenuItemId = item.MenuItemId,
                    Quantity = item.Quantity,
                    Price = item.Price // Ціна за одиницю на момент замовлення
                };
                order.Items.Add(orderItem);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Очищуємо кошик після успішного створення замовлення
_cartService.ClearCart();
            return RedirectToAction("Confirmation", new { orderId = order.Id });
        }

        // GET: /Order/Confirmation
        // Сторінка "Дякуємо за замовлення"
        public IActionResult Confirmation(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }
    }
}
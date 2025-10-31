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
    [Authorize]
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

        public IActionResult Checkout()
        {
            var cart = _cartService.GetCartViewModel();
            if (!cart.Items.Any())
            {
                return RedirectToAction("Index", "Cart");
            }
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrder(string deliveryAddress)
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
            };

            foreach (var item in cart.Items)
            {
                var orderItem = new OrderItem
                {
                    MenuItemId = item.MenuItemId,
                    Quantity = item.Quantity,
                    Price = item.Price
                };
                order.Items.Add(orderItem);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

_cartService.ClearCart();
            return RedirectToAction("Confirmation", new { orderId = order.Id });
        }

        public IActionResult Confirmation(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }
    }
}
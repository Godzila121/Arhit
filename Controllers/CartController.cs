using FoodDelivery.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FoodDelivery.Controllers 
{
    public class CartController : Controller
    {

        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        public IActionResult Index()
        {
            var cartViewModel = _cartService.GetCartViewModel();
            return View(cartViewModel);
        }

        public async Task<IActionResult> AddToCart(int menuItemId)
        {
            await _cartService.AddItemToCartAsync(menuItemId);

            return Redirect(Request.Headers["Referer"].ToString() ?? "/");
        }

        public IActionResult RemoveFromCart(int menuItemId)
        {
            _cartService.RemoveItemFromCart(menuItemId);
            return RedirectToAction("Index");
        }
    }
}
using FoodDelivery.Models;
using FoodDelivery.ViewModels;
using System.Text.Json;

namespace FoodDelivery.Services
{
    public class CartService
    {
        private const string CartSessionKey = "Cart";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession? Session => _httpContextAccessor.HttpContext?.Session;

        public CartViewModel GetCart()
        {
            var cartJson = Session?.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new CartViewModel(); // Повертаємо порожній кошик
            }
            return JsonSerializer.Deserialize<CartViewModel>(cartJson) ?? new CartViewModel();
        }

        public void SaveCart(CartViewModel cart)
        {
            cart.Total = cart.Items.Sum(item => item.MenuItem.Price * item.Quantity);
            var cartJson = JsonSerializer.Serialize(cart);
            Session?.SetString(CartSessionKey, cartJson);
        }

        public void AddItem(MenuItem menuItem)
        {
            var cart = GetCart();
            var cartItem = cart.Items.FirstOrDefault(i => i.MenuItem.Id == menuItem.Id);

            if (cartItem != null)
            {
                cartItem.Quantity++; // Якщо товар вже є, збільшуємо кількість
            }
            else
            {
                cart.Items.Add(new CartItemViewModel { MenuItem = menuItem, Quantity = 1 });
            }

            SaveCart(cart);
        }
    }
}
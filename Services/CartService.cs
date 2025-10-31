using FoodDelivery.Data;
using FoodDelivery.Models;
using FoodDelivery.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;
        private ISession Session => _httpContextAccessor.HttpContext.Session;

        public CartService(IHttpContextAccessor httpContextAccessor, AppDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        private const string CartKey = "ShoppingCart";

        public async Task AddItemToCartAsync(int menuItemId)
        {
            var menuItem = await _context.MenuItems
                                         .Include(m => m.Restaurant) 
                                         .FirstOrDefaultAsync(m => m.Id == menuItemId);
            if (menuItem == null)
            {
                return;
            }

            var cart = GetCartItems();

            if (cart.Any() && cart.First().RestaurantId != menuItem.RestaurantId)
            {
                cart.Clear();
            }

            var cartItem = cart.FirstOrDefault(item => item.MenuItemId == menuItemId);
            if (cartItem == null)
            {
                cart.Add(new CartItemViewModel
                {
                    MenuItemId = menuItem.Id,
                    Name = menuItem.Name,
                    Price = menuItem.Price,
                    Quantity = 1,
                    RestaurantId = menuItem.RestaurantId,
                    RestaurantName = menuItem.Restaurant.Name
                });
            }
            else
            {
                cartItem.Quantity++;
            }
            SaveCart(cart);
        }
        
        public void RemoveItemFromCart(int menuItemId)
        {
            var cart = GetCartItems();
            var cartItem = cart.FirstOrDefault(item => item.MenuItemId == menuItemId);
            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                }
                else
                {
                    cart.Remove(cartItem);
                }
            }
            SaveCart(cart);
        }

        public CartViewModel GetCartViewModel()
        {
            var cartItems = GetCartItems();
            return new CartViewModel
            {
                Items = cartItems,
                Total = cartItems.Sum(item => item.Price * item.Quantity)
            };
        }

        private List<CartItemViewModel> GetCartItems()
        {
            var cartJson = Session.GetString(CartKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItemViewModel>();
            }
            return JsonConvert.DeserializeObject<List<CartItemViewModel>>(cartJson);
        }

        private void SaveCart(List<CartItemViewModel> cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);
            Session.SetString(CartKey, cartJson);
        }
        public void ClearCart()
{
    SaveCart(new List<CartItemViewModel>());
}
    }
}
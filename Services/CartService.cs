using FoodDelivery.Data;
using FoodDelivery.Models;
using FoodDelivery.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json; // <-- Змінюємо серіалізатор на більш надійний
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context; // <-- 1. Додаємо доступ до бази даних
        private ISession Session => _httpContextAccessor.HttpContext.Session;

        // Оновлюємо конструктор, щоб сервіс міг працювати з базою даних
        public CartService(IHttpContextAccessor httpContextAccessor, AppDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        private const string CartKey = "ShoppingCart";

        // --- 2. РЕАЛІЗУЄМО AddItemToCartAsync ---
        public async Task AddItemToCartAsync(int menuItemId)
        {
            // Знаходимо товар в базі, одразу підвантажуючи дані про ресторан
            var menuItem = await _context.MenuItems
                                         .Include(m => m.Restaurant) 
                                         .FirstOrDefaultAsync(m => m.Id == menuItemId);
            if (menuItem == null)
            {
                return; // Якщо товар не знайдено, нічого не робимо
            }

            var cart = GetCartItems(); // Отримуємо поточний список товарів

            // Перевіряємо, чи можна додати товар з іншого ресторану
            if (cart.Any() && cart.First().RestaurantId != menuItem.RestaurantId)
            {
                // Якщо в кошику вже є товари з іншого ресторану, очищуємо його
                cart.Clear();
            }

            var cartItem = cart.FirstOrDefault(item => item.MenuItemId == menuItemId);
            if (cartItem == null)
            {
                // Якщо товару ще немає в кошику, додаємо його
                cart.Add(new CartItemViewModel
                {
                    MenuItemId = menuItem.Id,
                    Name = menuItem.Name,
                    Price = menuItem.Price,
                    Quantity = 1,
                    RestaurantId = menuItem.RestaurantId,
                    RestaurantName = menuItem.Restaurant.Name // Тепер це працюватиме
                });
            }
            else
            {
                // Якщо товар вже є, просто збільшуємо кількість
                cartItem.Quantity++;
            }
            SaveCart(cart); // Зберігаємо оновлений список
        }
        
        // --- 3. РЕАЛІЗУЄМО RemoveItemFromCart ---
        public void RemoveItemFromCart(int menuItemId)
        {
            var cart = GetCartItems();
            var cartItem = cart.FirstOrDefault(item => item.MenuItemId == menuItemId);
            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    // Якщо кількість більше 1, просто зменшуємо її
                    cartItem.Quantity--;
                }
                else
                {
                    // Якщо товар один, видаляємо його з кошика
                    cart.Remove(cartItem);
                }
            }
            SaveCart(cart);
        }

        // --- 4. РЕАЛІЗУЄМО GetCartViewModel ---
        public CartViewModel GetCartViewModel()
        {
            var cartItems = GetCartItems();
            // Створюємо ViewModel на основі даних з кошика
            return new CartViewModel
            {
                Items = cartItems,
                Total = cartItems.Sum(item => item.Price * item.Quantity)
            };
        }

        // --- Допоміжні методи для роботи з сесією ---

        // Цей метод тепер буде приватним і повертатиме лише список товарів
        private List<CartItemViewModel> GetCartItems()
        {
            var cartJson = Session.GetString(CartKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItemViewModel>();
            }
            // Використовуємо Newtonsoft.Json для десеріалізації
            return JsonConvert.DeserializeObject<List<CartItemViewModel>>(cartJson);
        }

        // Цей метод зберігає у сесію список товарів
        private void SaveCart(List<CartItemViewModel> cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);
            Session.SetString(CartKey, cartJson);
        }
        public void ClearCart()
{
    // Щоб очистити кошик, ми просто зберігаємо порожній список
    SaveCart(new List<CartItemViewModel>());
}
    }
}
using FoodDelivery.Data;
using FoodDelivery.Models;
using FoodDelivery.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly AppDbContext _context;

        public RestaurantController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Restaurant/Index
        // Цей метод буде показувати список всіх ресторанів
        public async Task<IActionResult> Index()
        {
            var restaurants = await _context.Restaurants.ToListAsync();
            return View(restaurants);
        }

        // GET: /Restaurant/Details/5
        // Цей метод покаже детальну інформацію про один ресторан
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurants
                .Include(r => r.MenuItems) // Включаємо меню
                .FirstOrDefaultAsync(m => m.Id == id);

            if (restaurant == null)
            {
                return NotFound();
            }
            
            // Створюємо ViewModel для передачі даних у представлення
            var viewModel = new RestaurantDetailsViewModel
            {
                Restaurant = restaurant,
                Reviews = new List<Review>(), // Тут буде логіка для відгуків
                NewReview = new Review()
            };

            return View(viewModel);
        }
    }
}
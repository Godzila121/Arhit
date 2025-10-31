using FoodDelivery.Data;
using FoodDelivery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FoodDelivery.Areas.RestaurantOwner.Controllers
{
    [Area("RestaurantOwner")]
    [Authorize(Roles = "RestaurantOwner, Admin")] 
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var restaurant = await _context.Restaurants
                                           .AsNoTracking()
                                           .FirstOrDefaultAsync(r => r.OwnerId == userId);

            if (restaurant == null)
            {
                return View("NoRestaurant"); 
            }
            
            return RedirectToAction("Index", "Menu", new { area = "RestaurantOwner", restaurantId = restaurant.Id });
        }
    }
}
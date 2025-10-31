using FoodDelivery.Data;
using FoodDelivery.Models;
using FoodDelivery.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<User> _userManager;

        public DashboardController(AppDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var totalOrders = await _db.Orders.CountAsync();
            var totalRestaurants = await _db.Restaurants.CountAsync();
            var averageRating = await _db.Reviews.AnyAsync() ? await _db.Reviews.AverageAsync(r => r.Rating) : 0;
            var totalUsers = await _db.Users.CountAsync();
            var totalCouriers = await _db.Couriers.CountAsync();
            var recentOrders = await _db.Orders
                                        .Include(o => o.User)
                                        .OrderByDescending(o => o.CreatedAt)
                                        .Take(10)
                                        .ToListAsync();

            var viewModel = new AdminDashboardViewModel
            {
                TotalOrders = totalOrders,
                TotalRestaurants = totalRestaurants,
                AverageRating = averageRating,
                TotalUsers = totalUsers,
                TotalCouriers = totalCouriers,
                RecentOrders = recentOrders
            };

            return View(viewModel);
        }
    }
}
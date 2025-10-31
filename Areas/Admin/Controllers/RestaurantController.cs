using FoodDelivery.Data;
using FoodDelivery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FoodDelivery.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RestaurantController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public RestaurantController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {

            var restaurants = await _context.Restaurants.Include(r => r.Owner).ToListAsync();
            return View(restaurants);
        }

        public async Task<IActionResult> Create()
        {
            var owners = await _userManager.GetUsersInRoleAsync("RestaurantOwner");

            ViewData["OwnerId"] = new SelectList(owners, "Id", "UserName");

            return View();
        }

public async Task<IActionResult> Details(int? id)
{
    if (id == null) return NotFound();
    
    var restaurant = await _context.Restaurants
        .Include(r => r.Owner)
        .Include(r => r.MenuItems)
        .FirstOrDefaultAsync(m => m.Id == id);
        
    if (restaurant == null) return NotFound();

    return View(restaurant);
}

public async Task<IActionResult> Edit(int? id)
{
    if (id == null) return NotFound();

    var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null) return NotFound();
    
    var owners = await _userManager.GetUsersInRoleAsync("RestaurantOwner");
    ViewData["OwnerId"] = new SelectList(owners, "Id", "UserName", restaurant.OwnerId);
    return View(restaurant);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Phone,Address,OwnerId")] Restaurant restaurant)
{
    if (id != restaurant.Id) return NotFound();

    if (ModelState.IsValid)
    {
        try
        {
            _context.Update(restaurant);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Restaurants.Any(e => e.Id == restaurant.Id)) return NotFound();
            else throw;
        }
        return RedirectToAction(nameof(Index));
    }
    
    var owners = await _userManager.GetUsersInRoleAsync("RestaurantOwner");
    ViewData["OwnerId"] = new SelectList(owners, "Id", "UserName", restaurant.OwnerId);
    return View(restaurant);
}

public async Task<IActionResult> Delete(int? id)
{
    if (id == null) return NotFound();

    var restaurant = await _context.Restaurants
        .Include(r => r.Owner)
        .FirstOrDefaultAsync(m => m.Id == id);
    if (restaurant == null) return NotFound();

    return View(restaurant);
}

[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    var restaurant = await _context.Restaurants.FindAsync(id);
    if (restaurant != null)
    {
        _context.Restaurants.Remove(restaurant);
    }

    await _context.SaveChangesAsync();
    return RedirectToAction(nameof(Index));
}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Phone,Address,OwnerId")] Restaurant restaurant)
        {
            if (ModelState.IsValid)
            {
                _context.Add(restaurant);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            var owners = await _userManager.GetUsersInRoleAsync("RestaurantOwner");
            ViewData["OwnerId"] = new SelectList(owners, "Id", "UserName", restaurant.OwnerId);

            return View(restaurant);
        }
    }
    
}
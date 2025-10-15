using FoodDelivery.Data; // Для AppDbContext
using FoodDelivery.Models; // Для ErrorViewModel
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FoodDelivery.Controllers;

public class HomeController : Controller
{
    // Обидва поля для залежностей
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _db;

    // Об'єднаний конструктор, який приймає обидві залежності
    public HomeController(ILogger<HomeController> logger, AppDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    // Головна дія (Index), що використовує базу даних для отримання ресторанів
    public IActionResult Index()
    {
        // Використовуємо логіку з другого файлу
        var restaurants = _db.Restaurants.Take(10).ToList();
        return View(restaurants); // Передаємо список ресторанів у представлення
    }

    // Сторінка політики конфіденційності
    public IActionResult Privacy()
    {
        return View();
    }

    // Дія обробки помилок
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

// !!! ВАЖЛИВО !!!
// Видаліть внутрішній клас ErrorViewModel, який ви помилково додали в перший файл.
// Модель ErrorViewModel має бути визначена окремо у файлі Models/ErrorViewModel.cs
// і виглядати так, як ми обговорювали в попередньому кроці:
/*
namespace FoodDelivery.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
*/
// ViewModels/AdminDashboardViewModel.cs
using FoodDelivery.Models; // <-- Важливо: додайте цей рядок
using System.Collections.Generic; // <-- і цей

namespace FoodDelivery.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalOrders { get; set; }
        public int TotalRestaurants { get; set; }
        public double AverageRating { get; set; }

        // --- Додані властивості ---
        public int TotalUsers { get; set; }
        public int TotalCouriers { get; set; }
        public List<Order> RecentOrders { get; set; } = new List<Order>(); // Ініціалізуємо список
    }
}
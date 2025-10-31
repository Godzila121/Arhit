using FoodDelivery.Models;
using System.Collections.Generic;

namespace FoodDelivery.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalOrders { get; set; }
        public int TotalRestaurants { get; set; }
        public double AverageRating { get; set; }

        public int TotalUsers { get; set; }
        public int TotalCouriers { get; set; }
        public List<Order> RecentOrders { get; set; } = new List<Order>();
    }
}
// ViewModels/AdminDashboardViewModel.cs
namespace FoodDelivery.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalOrders { get; set; }
        public int TotalRestaurants { get; set; }
        public double AverageRating { get; set; }
    }
}
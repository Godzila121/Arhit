using FoodDelivery.Models;

namespace FoodDelivery.ViewModels
{
    public class CartItemViewModel
    {
        public MenuItem MenuItem { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
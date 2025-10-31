namespace FoodDelivery.ViewModels
{
    public class CartItemViewModel
    {
        public int MenuItemId { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
    }
}
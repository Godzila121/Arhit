namespace FoodDelivery.ViewModels
{
    public class CartItemViewModel
    {
        // ID самого товару
        public int MenuItemId { get; set; }

        // Інформація для відображення
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        // Додаткова інформація для логіки кошика (щоб не можна було замовити з різних ресторанів)
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
    }
}
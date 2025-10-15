using FoodDelivery.Models;

public class RestaurantDetailsViewModel
{
    public Restaurant Restaurant { get; set; }
    public IEnumerable<Review> Reviews { get; set; }
    public Review NewReview { get; set; } // Для форми додавання нового відгуку
}
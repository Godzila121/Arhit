using FoodDelivery.Models;

public class RestaurantDetailsViewModel
{
    public Restaurant Restaurant { get; set; }= new();
    public IEnumerable<Review> Reviews { get; set; }
    public Review NewReview { get; set; }= new();
}
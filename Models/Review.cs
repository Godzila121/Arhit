namespace FoodDelivery.Models; // <--- ДОДАНО ЦЕЙ РЯДОК

public class Review
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
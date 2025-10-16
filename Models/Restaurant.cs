namespace FoodDelivery.Models; // <--- ДОДАНО ЦЕЙ РЯДОК

public class Restaurant
{
    public int Id { get; set; }
    public string Name { get; set; }= string.Empty;
    public string Description { get; set; }= string.Empty;
    public string Phone { get; set; }= string.Empty;
    public string Address { get; set; }= string.Empty;

    // --- ДОДАЙТЕ ЦІ РЯДКИ ---
    public string? OwnerId { get; set; } // Зв'язок з User
    public virtual User? Owner { get; set; }
    // -------------------------

    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
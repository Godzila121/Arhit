namespace FoodDelivery.Models; // <--- ДОДАНО ЦЕЙ РЯДОК

public enum OrderStatus { Pending, Accepted, InTransit, Delivered, Cancelled }

public class Order
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public User User { get; set; } = null!;
    public int? CourierId { get; set; }
    public Courier? Courier { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal Total { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
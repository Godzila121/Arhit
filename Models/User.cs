namespace FoodDelivery.Models; // <--- ДОДАНО ЦЕЙ РЯДОК
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic; // Можливо, знадобиться додати

public class User : IdentityUser
{
    // Ваші існуючі властивості, наприклад:
    public string? FullName { get; set; }
    public string? Address { get; set; }

    // --- ДОДАЙТЕ ЦЕЙ РЯДОК ---
    // Це зворотній зв'язок для Entity Framework
    public virtual ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
}
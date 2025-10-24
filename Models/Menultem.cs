using System.ComponentModel.DataAnnotations; // <-- Додайте цей рядок
using System.ComponentModel.DataAnnotations.Schema; // <-- Додайте цей рядок

namespace FoodDelivery.Models;

public class MenuItem
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; } = null!;

    [Required(ErrorMessage = "Назва страви є обов'язковою.")] // <-- Атрибут для Name
    public string Name { get; set; } = null!; 

    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ціна є обов'язковою.")] // <-- Атрибут для Price (обов'язковість)
    [Range(0.01, 10000, ErrorMessage = "Ціна повинна бути більшою за 0.")] // <-- Атрибут для Price (діапазон)
    [Column(TypeName = "decimal(18,2)")] // <-- Важливо для бази даних
    public decimal Price { get; set; }

    public bool IsAvailable { get; set; } = true;
}
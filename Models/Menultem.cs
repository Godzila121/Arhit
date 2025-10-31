using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Models;

public class MenuItem
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; } = null!;

    [Required(ErrorMessage = "Назва страви є обов'язковою.")]
    public string Name { get; set; } = null!; 

    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ціна є обов'язковою.")]
    [Range(0.01, 10000, ErrorMessage = "Ціна повинна бути більшою за 0.")]
    [Column(TypeName = "decimal(18,2)")]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }

    public bool IsAvailable { get; set; } = true;
}
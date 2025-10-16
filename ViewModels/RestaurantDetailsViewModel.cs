using FoodDelivery.Models;
using System.Collections.Generic;

namespace FoodDelivery.ViewModels
{
    public class RestaurantDetailsViewModel
    {
        public Restaurant Restaurant { get; set; } = new();
        
        public List<Review> Reviews { get; set; } = new();
        
        public Review NewReview { get; set; } = new();
    }
}
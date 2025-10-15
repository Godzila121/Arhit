namespace FoodDelivery.Models
{
    // Клас, який використовується для відображення деталей помилки.
    public class ErrorViewModel
    {
        // Унікальний ідентифікатор запиту, який використовується для відстеження помилки.
        public string? RequestId { get; set; }

        // Обчислювальна властивість, що показує, чи потрібно відображати RequestId.
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
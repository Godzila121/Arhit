public class Courier
{
public int Id { get; set; }
public string FullName { get; set; } = null!;
public string Phone { get; set; } = null!;
public bool IsActive { get; set; } = true;
public ICollection<Order> Orders { get; set; } = new List<Order>();
}
public class Restaurant
{
public int Id { get; set; }
public string Name { get; set; } = null!;
public string Address { get; set; } = null!;
public string Phone { get; set; } = null!;
public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
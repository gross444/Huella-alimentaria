namespace HuellaApi.Models;

public class Product
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = default!;
    public string Category { get; set; } = "Food";
    public string? Description { get; set; }
}

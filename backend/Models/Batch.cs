namespace HuellaApi.Models;

public class Batch
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string ProductId { get; set; } = default!;
    public string LotCode { get; set; } = default!;
    public DateTime ProducedAt { get; set; } = DateTime.UtcNow;
}

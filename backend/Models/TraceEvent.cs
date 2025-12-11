namespace HuellaApi.Models;

public class TraceEvent
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string BatchId { get; set; } = default!;
    public string Stage { get; set; } = default!; // Origen, Procesamiento, Transporte, Distribución
    public string Location { get; set; } = default!;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Data { get; set; } = "{}";

    // Cadena tipo blockchain
    public string PrevHash { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
}

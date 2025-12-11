using HuellaApi.Models;

namespace HuellaApi.Data;

public class InMemoryDb
{
    public List<Product> Products { get; } = new();
    public List<Batch> Batches { get; } = new();
    public List<TraceEvent> Events { get; } = new();
}

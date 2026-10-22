using Microsoft.EntityFrameworkCore;
using HuellaApi.Models; // Si tienes los modelos Product, Batch, TraceEvent

namespace HuellaApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tus tablas
        public DbSet<Product> Products { get; set; } = default!;
        public DbSet<Batch> Batches { get; set; } = default!;
        public DbSet<TraceEvent> TraceEvents { get; set; } = default!;
    }
}
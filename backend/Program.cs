using HuellaApi.Data;
using HuellaApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Si no hay ASPNETCORE_URLS (ejecución local con "dotnet run"), usamos el puerto 5000.
// En Docker, el Dockerfile ya define ASPNETCORE_URLS=http://+:8080, así que esto se omite.
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
{
    builder.WebHost.UseUrls("http://localhost:5000");
}

builder.Services.AddControllers();

// Base de datos en memoria (para pruebas; los datos se pierden al reiniciar)
builder.Services.AddSingleton<InMemoryDb>();

builder.Services.AddScoped<HashService>();
builder.Services.AddScoped<QrService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Sirve el frontend si lo copias a backend/wwwroot (opcional)
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

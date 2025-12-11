using HuellaApi.Data;
using HuellaApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Servicios
builder.Services.AddSingleton<InMemoryDb>();
builder.Services.AddSingleton<HashService>();
builder.Services.AddSingleton<QrService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Cabeceras de seguridad básicas (puedes reforzar con Nginx/Traefik)
app.Use(async (ctx, next) =>
{
    ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
    ctx.Response.Headers["X-Frame-Options"] = "DENY";
    ctx.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    await next();
});

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapControllers();

app.Run();

using Microsoft.EntityFrameworkCore;
using Rukia.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Controllers (necessário para /clientes via controller)
builder.Services.AddControllers();

// DbContext (services SEMPRE antes do Build)
var cs = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<RukiaDbContext>(opt =>
    opt.UseNpgsql(cs));

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

// Health DB (agora app já existe)
app.MapGet("/health/db", async (RukiaDbContext db) =>
{
    var ok = await db.Database.CanConnectAsync();
    return Results.Ok(new { db = ok });
});

// Controllers endpoints
app.MapControllers();

// (Opcional) manter seu endpoint legado de teste
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
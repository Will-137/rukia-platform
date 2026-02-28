using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rukia.Api.Errors;
using Rukia.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Controllers
builder.Services.AddControllers();

// DbContext
var cs = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<RukiaDbContext>(opt => opt.UseNpgsql(cs));

// Middleware de exceção (DI)
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

// Padroniza resposta de validação automática (model binding / data annotations)
builder.Services.Configure<ApiBehaviorOptions>(opt =>
{
    opt.InvalidModelStateResponseFactory = ctx =>
    {
        var vpd = ApiProblemDetails.CreateValidation(
            ctx.ModelState,
            ErrorCodes.ValidacaoRequisicaoInvalida
        );

        return new BadRequestObjectResult(vpd);
    };
});

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware global de exceção (antes de mapear endpoints)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Health DB
app.MapGet("/health/db", async (RukiaDbContext db) =>
{
    var ok = await db.Database.CanConnectAsync();
    return Results.Ok(new { db = ok });
});

// Controllers endpoints
app.MapControllers();

// (Opcional) manter endpoint legado de teste
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

public partial class Program { }

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
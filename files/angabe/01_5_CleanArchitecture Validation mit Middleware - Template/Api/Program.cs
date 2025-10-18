using Application;
using Infrastructure;
using Microsoft.OpenApi.Models;
using Api.Middleware;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Pfad zur CSV-Datei (im Ausgabeverzeichnis erwartet)
        var csvPath = Path.Combine(AppContext.BaseDirectory, "measurements.csv");
        // DB-Connection aus appsettings.json (Default). Fällt sonst auf LocalDB zurück (siehe Infrastructure.AddInfrastructure)
        var connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new ArgumentException("Connection string not found");
        // Registriert Infrastruktur (DbContext, Repositories, UoW, CSV-Reader, Seeder)
        builder.Services.AddInfrastructure(csvPath, connectionString);
        builder.Services.AddApplication();
        // Web API Basics
        builder.Services.AddControllers();
        // Swagger/OpenAPI für einfache Erkundung der API
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Measurement API",
                Version = "v1",
                Description = "API zum Auslesen von Sensoren und Messwerten aus einer CSV"
            });
        });
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Measurement API v1");
                c.RoutePrefix = "swagger";
                c.DisplayRequestDuration();
            });
        }

        app.UseMiddleware<ValidationExceptionMiddleware>();
        app.UseHttpsRedirection();
        app.MapControllers();

        app.Run();
    }
}
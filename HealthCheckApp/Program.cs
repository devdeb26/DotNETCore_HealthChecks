using System.Text.Json;
using HealthCheckApp.Helpers;
using HealthCheckApp.Models.Dtos;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var healthChecksConfigs = builder.Configuration.GetSection("HealthChecks").Get<HealthChecksDto>();
var healthCheckServices = builder.Services.AddHealthChecks();

if (healthChecksConfigs.Databases != null)
{
    healthChecksConfigs.Databases.Where(x => x.Enabled).ToList().ForEach(db =>
    {
        healthCheckServices.AddSqlServer(builder.Configuration.GetConnectionString(db.Name), "", null, db.Name, HealthStatus.Unhealthy);
    });
}

if (healthChecksConfigs.URIs != null)
{
    healthChecksConfigs.URIs.Where(x => x.Enabled).ToList().ForEach(u =>
    {
        healthCheckServices.AddUrlGroup(new Uri(u.Name), u.Name, HealthStatus.Unhealthy);
    });
}

if (healthChecksConfigs.OtherConnections != null)
{
    healthChecksConfigs.OtherConnections.Where(x => x.Enabled).ToList().ForEach(o =>
    {
        switch (o.Name)
        {
            case "SendGrid": 
                healthCheckServices.AddCheck<SendGridHealthCheck>(o.Name);
                break;
        }
    });
}


var app = builder.Build();


app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            details = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            })
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
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

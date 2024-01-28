using AzureStorageDemo.Interfaces;
using AzureStorageDemo.Services;
using AzureStorageDemo.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAzureStorageService, AzureStorageService>(provider =>
{
    var connectionString = builder.Configuration.GetValue<string>("AzureStorage:ConnectionString");
    return new AzureStorageService(connectionString!);
});

var app = builder.Build();

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

app.MapPost("/weatherforecast", async (IAzureStorageService azureStorageService) =>
    {
        await azureStorageService.AddQueueMessageAsync(QueueNames.WeatherForecast, 
            new WeatherForecast(new DateOnly(2024,1,28), 0, "Cloud with a chance of meatballs."));
    })
    .WithName("CreateWeatherForecast")
    .WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
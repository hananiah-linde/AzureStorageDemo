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

app.MapPost("/queue", async (IAzureStorageService azureStorageService, object request) =>
    {
        await azureStorageService.AddQueueMessageAsync(QueueNames.WeatherForecast, request);
    })
    .WithName("CreateWeatherForecast")
    .WithOpenApi();

app.Run();

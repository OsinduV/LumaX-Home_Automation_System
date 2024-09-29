using backend.Services;
using MQTTnet;
using MQTTnet.Client;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Configure logging
builder.Logging.ClearProviders(); // Optionally clear default providers
builder.Logging.AddConsole();

builder.Services.AddSingleton<MqttService>();

builder.Services.AddSignalR(); // Add SignalR

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS to allow specific origin (Angular app) with credentials
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:4200")  // Angular app URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();  // This enables credential-based requests
    });
});

var app = builder.Build();

// Enable CORS
app.UseCors("AllowSpecificOrigin");  // Use the CORS policy defined above

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseAuthorization();

// Map SignalR hubs
app.MapHub<SignalRHub>("/temperatureHub");

app.MapControllers();


// Start the MQTT client when the app starts.
using (var scope = app.Services.CreateScope())
{
    var mqttService = scope.ServiceProvider.GetRequiredService<MqttService>();
    await mqttService.ConnectAsync(); // Connect to the MQTT broker on application startup.
}

app.Run();

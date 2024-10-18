using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

//services from Identity core
builder.Services
    .AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
});

var connectionString = builder.Configuration.GetConnectionString("DevDB");
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS to allow specific origin (Angular app) with credentials
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificOrigin", policy =>
//    {
//        policy.WithOrigins("http://localhost:4200")  // Angular app URL
//              .AllowAnyHeader()
//              .AllowAnyMethod()
//              .AllowCredentials();  // This enables credential-based requests
//    });
//});

var app = builder.Build();

// Enable CORS
//app.UseCors("AllowSpecificOrigin");  // Use the CORS policy defined above
#region Config. CORS
app.UseCors(options =>
    options.WithOrigins("http://localhost:4200")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
);
#endregion

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

app
    .MapGroup("/api")
    .MapIdentityApi<AppUser>();

app.MapPost("/api/signup", async (
    UserManager<AppUser> userManager,
    [FromBody] UserRegistrationModel userRegistrationModel
    ) =>
{
    AppUser user = new AppUser()
    {
        UserName = userRegistrationModel.Email,
        Email = userRegistrationModel.Email,
        FullName = userRegistrationModel.FullName,
    };
    var result = await userManager.CreateAsync(user, userRegistrationModel.Password);
    if (result.Succeeded)
        return Results.Ok(result);
    else 
        return Results.BadRequest(result);
});


// Start the MQTT client when the app starts.
//using (var scope = app.Services.CreateScope())
//{
//    var mqttService = scope.ServiceProvider.GetRequiredService<MqttService>();
//    await mqttService.ConnectAsync(); // Connect to the MQTT broker on application startup.
//}

app.Run();

public class UserRegistrationModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
}

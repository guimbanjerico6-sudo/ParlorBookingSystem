using Microsoft.EntityFrameworkCore;
using ParlorBookingSystem.Models;

var builder = WebApplication.CreateBuilder(args);

// Tells the app to use SQL Server and grab the password/location from appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllers();

// --- 1. THE SWAGGER FIX ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // --- 2. THE SWAGGER FIX ---
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// --- SEED DATABASE ON STARTUP ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    DbSeeder.Seed(services);
}

app.Run();
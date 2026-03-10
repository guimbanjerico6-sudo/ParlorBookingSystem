using Microsoft.EntityFrameworkCore;
using ParlorBookingSystem.Data;
using ParlorBookingSystem.Models;
using ParlorBookingSystem.Repositories;
using ParlorBookingSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Tells the app to use SQL Server and grab the password/location from appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IEmailService, EmailService>();
// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // This tells the API: "If you see a loop, just ignore it and stop."
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

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
app.UseStaticFiles();
app.MapControllers();

// --- SEED DATABASE ON STARTUP ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    DbSeeder.Seed(services);
}

app.Run();
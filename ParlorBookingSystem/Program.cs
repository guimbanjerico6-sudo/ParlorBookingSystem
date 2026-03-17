using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ParlorBookingSystem.Data;
using ParlorBookingSystem.Repositories;
using ParlorBookingSystem.Services;
using System.Text;
using System.Text.Json.Serialization; // <-- Required for fixing the JSON loop

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// --- THIS IS THE FIX FOR THE 500 ERROR ---
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();

// --- CORS FIX: Let React talk to C# ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Your Vite React port
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --- SWAGGER SETUP (Simple version to guarantee 0 errors) ---
builder.Services.AddSwaggerGen();

// Database Context Setup
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 1. IDENTITY SETUP: Manages users, passwords, and roles
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 2. JWT AUTHENTICATION SETUP: The Bouncer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "AuntiesParlor",
        ValidAudience = "AuntiesParlorUsers",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKeyForAuntiesParlor123!"))
    };
});

// Dependency Injection Setup (The Engines)
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

// --- MIDDLEWARE ORDER (CRITICAL) ---
app.UseAuthentication(); // 1. Identify WHO is calling
app.UseAuthorization();  // 2. Check WHAT they are allowed to do

app.MapControllers();

app.Run();
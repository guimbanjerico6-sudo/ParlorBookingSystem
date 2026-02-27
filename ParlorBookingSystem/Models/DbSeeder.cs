using Microsoft.EntityFrameworkCore;

namespace ParlorBookingSystem.Models
{
    public static class DbSeeder
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            // Connect to the database
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            // 1. Check if we already have Services. If yes, stop the script.
            if (context.Services.Any())
            {
                return;   // DB has been seeded
            }

            // 2. Inject the Aunt's Menu
            var services = new Service[]
            {
                new Service { Name = "Basic Haircut", DurationMinutes = 45, Price = 350.00m },
                new Service { Name = "Hair Color (Single Tone)", DurationMinutes = 120, Price = 1500.00m },
                new Service { Name = "Rebond with Treatment", DurationMinutes = 240, Price = 3000.00m }
            };
            context.Services.AddRange(services);

            // 3. Inject a Fake Customer
            var customer = new Customer
            {
                Name = "Maria Clara",
                PhoneNumber = "0917-123-4567",
                MessengerLink = "m.me/mariaclara"
            };
            context.Customers.Add(customer);

            // 4. Hit "Save" to push them to SQL
            context.SaveChanges();
        }
    }
}
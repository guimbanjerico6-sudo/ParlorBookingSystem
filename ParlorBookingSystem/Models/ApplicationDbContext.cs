using Microsoft.EntityFrameworkCore;
using ParlorBookingSystem.Models; // Make sure this using statement is at the top!

namespace ParlorBookingSystem.Data // Your namespace might be slightly different, that is okay
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // These three lines are the crucial part!
        // They tell EF Core to turn our C# Models into SQL Tables.
        public DbSet<User> Users { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
    }
}
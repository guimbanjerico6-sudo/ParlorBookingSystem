using Microsoft.EntityFrameworkCore;

namespace ParlorBookingSystem.Models
{
    // Inheriting from DbContext gives this class its translation powers
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // These DbSets are the actual tables that will be created in your database
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
    }
}
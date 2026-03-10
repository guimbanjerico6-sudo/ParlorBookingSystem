using Microsoft.EntityFrameworkCore;
using ParlorBookingSystem.Data;
using ParlorBookingSystem.Models;

namespace ParlorBookingSystem.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _context.Appointments.FindAsync(id);
        }
        // Put this right below your GetByIdAsync method
        public async Task<IEnumerable<Appointment>> GetAppointmentsForReviewAsync()
        {
            return await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Service)
                .Where(a => a.Status == "Payment Under Review")
                .ToListAsync();
        }

        public async Task<bool> HasOverlappingAppointmentsAsync(DateTime start, DateTime end)
        {
            // Calculate exactly what time it was 15 minutes ago
            var fifteenMinutesAgo = DateTime.Now.AddMinutes(-15);

            return await _context.Appointments.AnyAsync(a =>
                start < a.EstimatedEndTime &&
                end > a.RequestedStartTime &&
                (
                    a.Status == "Confirmed" ||
                    a.Status == "Payment Under Review" ||
                    (a.Status == "Awaiting Payment" && a.CreatedAt >= fifteenMinutesAgo)
                ));
        }

        public async Task AddAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
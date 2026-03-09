using ParlorBookingSystem.Models;

namespace ParlorBookingSystem.Repositories
{
    public interface IAppointmentRepository
    {
        Task<Appointment?> GetByIdAsync(int id);
        Task<bool> HasOverlappingAppointmentsAsync(DateTime start, DateTime end);
        Task AddAsync(Appointment appointment);
        Task SaveChangesAsync();
        Task<IEnumerable<Appointment>> GetAppointmentsForReviewAsync();
    }
}
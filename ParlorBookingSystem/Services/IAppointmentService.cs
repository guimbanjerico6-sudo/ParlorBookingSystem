using Microsoft.AspNetCore.Http; // We need this for IFormFile
using ParlorBookingSystem.Models;

namespace ParlorBookingSystem.Services
{
    public interface IAppointmentService
    {
        Task<Appointment> CreateAppointmentAsync(Appointment appointment);
        // Add our new Upload method contract!
        Task<string> UploadReceiptAsync(int appointmentId, IFormFile file);
        Task<IEnumerable<Appointment>> GetAppointmentsForReviewAsync();
        Task<Appointment> ConfirmAppointmentAsync(int appointmentId);
    }
}
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using ParlorBookingSystem.Models;
using ParlorBookingSystem.Repositories;
using ParlorBookingSystem.Data;

namespace ParlorBookingSystem.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AppointmentService(IAppointmentRepository appointmentRepo, ApplicationDbContext context, IWebHostEnvironment env)
        {
            _appointmentRepo = appointmentRepo;
            _context = context;
            _env = env;
        }

        public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
        {
            var parlorService = await _context.Services.FindAsync(appointment.ServiceId);
            if (parlorService == null) throw new Exception("Service not found.");

            int totalMinutes = parlorService.DurationMinutes + parlorService.BufferTimeMinutes;
            appointment.EstimatedEndTime = appointment.RequestedStartTime.AddMinutes(totalMinutes);

            bool isClashing = await _appointmentRepo.HasOverlappingAppointmentsAsync(appointment.RequestedStartTime, appointment.EstimatedEndTime);
            if (isClashing) throw new Exception("Sorry, Auntie is already booked for this time!");

            // Default to Awaiting Payment
            appointment.Status = "Awaiting Payment";

            await _appointmentRepo.AddAsync(appointment);
            await _appointmentRepo.SaveChangesAsync();
            return appointment;
        }

        public async Task<string> UploadReceiptAsync(int appointmentId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file was uploaded.");

            var appointment = await _appointmentRepo.GetByIdAsync(appointmentId);
            if (appointment == null)
                throw new Exception("Appointment not found.");

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

            // Safety check: Create the folder if it doesn't exist!
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            appointment.ReceiptImageUrl = "/uploads/" + uniqueFileName;

            // Move it to the next phase for Auntie!
            appointment.Status = "Payment Under Review";

            await _appointmentRepo.SaveChangesAsync();

            return appointment.ReceiptImageUrl;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsForReviewAsync()
        {
            // Just pass the request down to the Repository
            return await _appointmentRepo.GetAppointmentsForReviewAsync();
        }

        public async Task<Appointment> ConfirmAppointmentAsync(int appointmentId)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(appointmentId);
            if (appointment == null)
                throw new Exception("Appointment not found.");

            // Business Rule: Auntie can only confirm if they paid!
            if (appointment.Status != "Payment Under Review")
                throw new Exception("You can only confirm appointments that have uploaded a deposit receipt.");

            // Change the status to lock it in permanently
            appointment.Status = "Confirmed";

            await _appointmentRepo.SaveChangesAsync();

            return appointment;
        }
    }

}
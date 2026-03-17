using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using ParlorBookingSystem.Models;
using ParlorBookingSystem.Repositories;
using ParlorBookingSystem.Data;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Appointment> CreateAppointmentAsync(Appointment newAppointment)
        {
            // 1. Define the "Lock" Timer (15 minutes ago)
            var expirationTime = DateTime.Now.AddMinutes(-15);

            // 2. The Smart Database Check
            // We check if ANY appointment exists for this exact time that is either:
            // - Fully Approved
            // - Under Review (Paid, waiting for Auntie)
            // - Pending, BUT created less than 15 minutes ago (Holding the slot)
            var isSlotTaken = await _context.Appointments
                .AnyAsync(a =>
                    a.RequestedStartTime == newAppointment.RequestedStartTime &&
                    (
                        a.Status == "Approved" ||
                        a.Status == "Payment Under Review" ||
                        (a.Status == "Pending" && a.CreatedAt >= expirationTime)
                    )
                );

            // 3. Block the Double Booking!
            if (isSlotTaken)
            {
                throw new Exception("Sorry! This time slot is currently locked by another customer. Please choose a different time or try again in 15 minutes.");
            }

            // 4. If the coast is clear, save the new appointment as 'Pending'
            newAppointment.Status = "Pending";
            newAppointment.CreatedAt = DateTime.Now;

            _context.Appointments.Add(newAppointment);
            await _context.SaveChangesAsync();

            return newAppointment;
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
        public async Task<Appointment> RejectAppointmentAsync(int appointmentId)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(appointmentId);
            if (appointment == null)
                throw new Exception("Appointment not found.");

            appointment.Status = "Confirmed";

            await _appointmentRepo.SaveChangesAsync();

            return appointment;
        }
    }

}
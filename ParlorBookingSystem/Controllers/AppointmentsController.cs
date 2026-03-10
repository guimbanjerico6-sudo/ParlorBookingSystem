using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Needed for IFormFile
using ParlorBookingSystem.Models;
using ParlorBookingSystem.Services;

namespace ParlorBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        // We ONLY inject the Service layer here. No more _context!
        private readonly IAppointmentService _appointmentService;
        private readonly IEmailService _emailService;

        public AppointmentsController(IAppointmentService appointmentService, IEmailService emailService)
        {
            _appointmentService = appointmentService;
            _emailService = emailService;
        }

        // --- 1. THE CUSTOMER SIDE: Request an Appointment ---
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] Appointment appointmentRequest)
        {
            try
            {
                // Look how beautifully clean this is now! 
                // The Bouncer math, the Buffer Time, and the Database saves are ALL handled by the Service.
                var createdAppointment = await _appointmentService.CreateAppointmentAsync(appointmentRequest);

                return Ok(new
                {
                    Message = "Appointment requested successfully! Pending Auntie's deposit verification.",
                    Appointment = createdAppointment
                });
            }
            catch (Exception ex)
            {
                // If the Bouncer finds a clash, the Service throws an error, and we catch it here.
                return BadRequest(new { Error = ex.Message });
            }
        }

            // 3. THE BOUNCER: Check for overlaps ONLY with "Confirmed" appointments
            // This logic checks if the new time starts before another ends, 
            // AND ends after another starts.
            bool isClashing = await _context.Appointments.AnyAsync(a =>
                a.Status == "Confirmed" &&
                appointment.RequestedStartTime < a.EstimatedEndTime &&
                appointment.EstimatedEndTime > a.RequestedStartTime);

            if (isClashing)
            {
                return BadRequest("Sorry, Auntie is already booked for a confirmed service at this time.");
            }

            // 4. Set default status to "Pending" so Auntie can review her errands
            appointment.Status = "Pending";

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return Ok(appointment);
        }

        // --- THE AUNTIE SIDE: Get all Pending requests for her Inbox ---
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetPendingAppointments()
        {
            return await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Customer)
                .Where(a => a.Status == "Pending")
                .ToListAsync();
        }
    }
}
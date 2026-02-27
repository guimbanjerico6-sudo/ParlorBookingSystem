using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParlorBookingSystem.Models;

namespace ParlorBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- THE CUSTOMER SIDE: Request an Appointment ---
        [HttpPost]
        public async Task<ActionResult<Appointment>> PostAppointment(Appointment appointment)
        {
            // 1. Find the service to get its duration (e.g., 45 mins)
            var service = await _context.Services.FindAsync(appointment.ServiceId);
            if (service == null) return BadRequest("Service not found.");

            // 2. Automatically calculate the end time based on the service duration
            appointment.EstimatedEndTime = appointment.RequestedStartTime.AddMinutes(service.DurationMinutes);

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
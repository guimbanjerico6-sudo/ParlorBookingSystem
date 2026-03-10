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

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
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

        // --- 2. THE CUSTOMER SIDE: Upload Deposit Receipt ---
        [HttpPost("{id}/receipt")]
        public async Task<IActionResult> UploadReceipt(int id, IFormFile file)
        {
            try
            {
                var fileUrl = await _appointmentService.UploadReceiptAsync(id, file);

                return Ok(new
                {
                    Message = "Receipt uploaded successfully!",
                    ImageUrl = fileUrl
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        /* * ========================================================
         * COMING SOON: AUNTIE'S DASHBOARD (Option 2 on our Sprint)
         * We will rebuild these the N-Tier way next!
         * ========================================================
         *
         * [HttpGet("pending")]
         * public async Task<IActionResult> GetPendingAppointments() { ... }
         * * [HttpPut("confirm/{id}")]
         * public async Task<IActionResult> ConfirmAppointment(int id) { ... }
         */

        // --- 3. THE AUNTIE SIDE: View Inbox ---
        // GET: api/Appointments/review
        [HttpGet("review")]
        public async Task<IActionResult> GetAppointmentsForReview()
        {
            var appointments = await _appointmentService.GetAppointmentsForReviewAsync();
            return Ok(appointments);
        }

        // --- 4. THE AUNTIE SIDE: Accept an Appointment ---
        // PUT: api/Appointments/5/confirm
        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> ConfirmAppointment(int id)
        {
            try
            {
                var confirmedAppointment = await _appointmentService.ConfirmAppointmentAsync(id);
                return Ok(new
                {
                    Message = "Appointment officially CONFIRMED! The Bouncer is now protecting this slot permanently.",
                    Appointment = confirmedAppointment
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
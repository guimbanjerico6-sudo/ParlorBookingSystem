using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ParlorBookingSystem.Models;
using ParlorBookingSystem.Services;
using Microsoft.AspNetCore.Authorization;
namespace ParlorBookingSystem.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        // Notice: We ONLY inject the Service layers here. No _context allowed!
        private readonly IAppointmentService _appointmentService;
        private readonly IEmailService _emailService;

        public AppointmentsController(IAppointmentService appointmentService, IEmailService emailService)
        {
            _appointmentService = appointmentService;
            _emailService = emailService;
        }

        // --- 1. THE CUSTOMER SIDE: Book a Slot ---
        // POST: api/Appointments
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] Appointment appointment)
        {
            try
            {
                var newAppointment = await _appointmentService.CreateAppointmentAsync(appointment);
                return Ok(newAppointment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // --- 2. THE CUSTOMER SIDE: Upload GCash Receipt ---
        // POST: api/Appointments/5/receipt
        [HttpPost("{id}/receipt")]
        public async Task<IActionResult> UploadReceipt(int id, IFormFile file)
        {
            try
            {
                var updatedAppointment = await _appointmentService.UploadReceiptAsync(id, file);
                return Ok(new
                {
                    Message = "Receipt uploaded successfully!",
                    Appointment = updatedAppointment
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // --- 3. THE AUNTIE SIDE: View Inbox ---
        // GET: api/Appointments/review
        [Authorize(Roles = "Admin")]
        [HttpGet("review")]
        public async Task<IActionResult> GetAppointmentsForReview()
        {
            var appointments = await _appointmentService.GetAppointmentsForReviewAsync();
            return Ok(appointments);
        }

        // --- 4. THE AUNTIE SIDE: Accept an Appointment ---
        // PUT: api/Appointments/5/confirm
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> ConfirmAppointment(int id)
        {
            try
            {
                // 1. Lock it in the database
                var confirmedAppointment = await _appointmentService.ConfirmAppointmentAsync(id);

                // 2. Draft the automated email
                // IMPORTANT: Put your actual email here for testing!
                // Use the email of the person who actually booked the appointment!
                // Just for testing, use a hardcoded string until we fix the Model relationship
                // Use '.Customer' because that is the name in your Appointment model!
                string targetEmail = confirmedAppointment.Customer?.Email ?? "ekobmln1@gmail.com";
                string subject = "Appointment Confirmed - Auntie's Parlor";

                string body = $@"
                    <h2>Good News!</h2>
                    <p>Auntie has successfully received your deposit.</p>
                    <p>Your appointment for <strong>{confirmedAppointment.RequestedStartTime:f}</strong> is officially locked in.</p>
                    <p>See you soon!</p>";

                // 3. Fire the email
                await _emailService.SendEmailAsync(targetEmail, subject, body);

                return Ok(new
                {
                    Message = "Appointment CONFIRMED and Email Sent successfully!",
                    Appointment = confirmedAppointment
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // --- 5. THE AUNTIE SIDE: Reject an Appointment ---
        // PUT: api/Appointments/5/reject
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> RejectAppointment(int id)
        {
            try
            {
                // 1. Update the database 
                // (Note: Make sure RejectAppointmentAsync exists in your AppointmentService!)
                var rejectedAppointment = await _appointmentService.RejectAppointmentAsync(id);

                // 2. Draft the apology email
                string targetEmail = rejectedAppointment.Customer?.Email ?? "ekobmln1@gmail.com";
                string subject = "Appointment Update - Auntie's Parlor";

                string body = $@"
                    <h2>Hello {rejectedAppointment.Customer?.FullName ?? "there"},</h2>
                    <p>We are sorry, but Auntie cannot accommodate your appointment request for <strong>{rejectedAppointment.RequestedStartTime:f}</strong> at this time.</p>
                    <p>If you sent a deposit, we will initiate a refund shortly. Please message the page to reschedule!</p>";

                // 3. Fire the email
                await _emailService.SendEmailAsync(targetEmail, subject, body);

                return Ok(new
                {
                    Message = "Appointment REJECTED and Email Sent successfully!",
                    Appointment = rejectedAppointment
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
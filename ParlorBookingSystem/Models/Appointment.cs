using System;

namespace ParlorBookingSystem.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ServiceId { get; set; }
        public DateTime RequestedStartTime { get; set; }
        public DateTime EstimatedEndTime { get; set; }

        // 1. Changed default from "Pending" to "Awaiting Payment"
        public string Status { get; set; } = "Awaiting Payment";

        public string? ReceiptImageUrl { get; set; }

        // 2. THE TIMER: Records the exact moment the appointment is placed
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public User? Customer { get; set; }
        public Service? Service { get; set; }
    }
}
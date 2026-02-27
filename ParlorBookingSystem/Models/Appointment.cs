namespace ParlorBookingSystem.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        // --- FOREIGN KEYS ---
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public int ServiceId { get; set; }
        public Service? Service { get; set; }

        // --- TIME ---
        public DateTime RequestedStartTime { get; set; }
        public DateTime EstimatedEndTime { get; set; } // Calculated later using Service.DurationMinutes

        // Optional notes from the customer (e.g., "Thick hair")
        public string? SpecialNotes { get; set; }

        // --- THE INBOX STATE MACHINE ---
        // Allowed values: "Pending", "Awaiting Payment", "Confirmed", "Completed", "Declined"
        public string Status { get; set; } = "Pending";

        // --- THE MONEY TRACKER ---
        // Allowed values: "Unpaid", "Deposit Paid", "Fully Paid"
        public string PaymentStatus { get; set; } = "Unpaid";

        public decimal AmountPaid { get; set; } = 0;

        // Nullable (?) because they don't have a GCash receipt number when they first request it
        public string? PaymentReferenceId { get; set; }
    }
}
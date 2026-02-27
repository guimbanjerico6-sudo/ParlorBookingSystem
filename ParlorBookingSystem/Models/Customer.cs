namespace ParlorBookingSystem.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Essential for the Aunt to reply via her current workflow
        public string MessengerLink { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        // EF Core Navigation: One Customer can have many Appointments
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
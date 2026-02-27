namespace ParlorBookingSystem.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Critical for the Bouncer to calculate when the appointment ends
        public int DurationMinutes { get; set; }

        // Using 'decimal' is a strict rule in C# when dealing with money!
        public decimal Price { get; set; }

        // EF Core Navigation: One Service can be booked in many Appointments
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
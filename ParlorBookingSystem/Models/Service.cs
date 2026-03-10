using System;
using System.Collections.Generic;

namespace ParlorBookingSystem.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }

        // Our new business rule requirement!
        public int BufferTimeMinutes { get; set; } = 15;

        // Navigation Property: One Service can have many Appointments
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
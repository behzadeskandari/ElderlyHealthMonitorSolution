using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElderlyHealthMonitor.Domain.Entities
{
    public class Alert
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? EventId { get; set; }
        public Guid ElderlyProfileId { get; set; }
        public Guid? CaregiverId { get; set; }
        public Guid UserId { get; set; } // Caregiver
        public DateTime SentAtUtc { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Open"; // Open/Acked/Closed
        public string Channel { get; set; } = "Push"; // Push/SMS/Call
        public string Message { get; set; } = string.Empty;
        public Event? Event { get; set; }
        public ElderlyProfile ElderlyProfile { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}

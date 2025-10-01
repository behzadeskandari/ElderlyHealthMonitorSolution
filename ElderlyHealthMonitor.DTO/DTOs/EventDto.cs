using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Domain.Enums;

namespace ElderlyHealthMonitor.Application.DTOs
{
    public class EventDto
    {
        public Guid ElderlyProfileId { get; set; }
        public EventType EventType { get; set; }
        public string? Source { get; set; }
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
        public AlertSeverity Severity { get; set; } = AlertSeverity.Medium;
        public string? DetailsJson { get; set; }
    }
}

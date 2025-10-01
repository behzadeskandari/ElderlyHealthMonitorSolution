using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElderlyHealthMonitor.Domain.Entities
{
    public class Event
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ElderlyProfileId { get; set; }
        [Required] public string EventType { get; set; } = string.Empty; // "Fall", "HighHR"
        public string Source { get; set; } = "Edge"; // Edge/ML/Server
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
        public int Severity { get; set; } = 1; // 1=Low, 3=High
        public string DetailsJson { get; set; } = string.Empty;
        public ElderlyProfile ElderlyProfile { get; set; } = null!;
        public Alert? Alert { get; set; }
    }
}

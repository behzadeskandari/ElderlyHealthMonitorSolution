using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElderlyHealthMonitor.Domain.Entities
{
    public class MedicationSchedule
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ElderlyProfileId { get; set; }
        [Required][MaxLength(100)] public string MedicationName { get; set; } = string.Empty;
        public string Dose { get; set; } = string.Empty;
        public string ScheduleJson { get; set; } = string.Empty; // e.g., {"times": ["08:00", "20:00"]}
        public DateTime? LastTakenAt { get; set; }
        public int MissedCount { get; set; } = 0;
        public ElderlyProfile ElderlyProfile { get; set; } = null!;
    }
}

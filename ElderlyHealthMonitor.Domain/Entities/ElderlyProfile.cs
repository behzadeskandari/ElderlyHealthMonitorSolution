using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElderlyHealthMonitor.Domain.Entities
{
    public class ElderlyProfile
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; } // Caregiver owner
        [Required][MaxLength(100)] public string Name { get; set; } = string.Empty;
        public DateTime? DOB { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string MedicalNotes { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public ICollection<Device> Devices { get; set; } = new List<Device>();
        public ICollection<SensorReading> SensorReadings { get; set; } = new List<SensorReading>();
        public ICollection<MedicationSchedule> MedicationSchedules { get; set; } = new List<MedicationSchedule>();
        public ICollection<BehavioralPattern> BehavioralPatterns { get; set; } = new List<BehavioralPattern>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

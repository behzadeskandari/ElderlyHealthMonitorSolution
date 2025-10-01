using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElderlyHealthMonitor.Domain.Entities
{
    public class SensorReading
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();
        public Guid DeviceId { get; set; }
        public Guid ElderlyProfileId { get; set; }
        [Required][MaxLength(50)] public string SensorType { get; set; } = string.Empty; // "HeartRate", "Accelerometer"
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
        public double? ValueDouble { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string PayloadJson { get; set; } = string.Empty; // Raw data as JSON
        public bool Processed { get; set; } = false;
        public double? AnomalyScore { get; set; } // From ML
        public Device Device { get; set; } = null!;
        public ElderlyProfile ElderlyProfile { get; set; } = null!;
    }
}

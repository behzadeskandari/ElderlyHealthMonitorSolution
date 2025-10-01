using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElderlyHealthMonitor.Domain.Entities
{
    public class Device
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ElderlyProfileId { get; set; }
        [Required][MaxLength(50)] public string DeviceType { get; set; } = string.Empty; // e.g., "Wristband", "Gateway"
        [Required][MaxLength(100)] public string Identifier { get; set; } = string.Empty; // MAC/BLE address
        public string FirmwareVersion { get; set; } = string.Empty;
        public DateTime? LastSeenUtc { get; set; }
        public bool IsActive { get; set; } = true;
        public ElderlyProfile ElderlyProfile { get; set; } = null!;
        public ICollection<SensorReading> SensorReadings { get; set; } = new List<SensorReading>();
    }
}

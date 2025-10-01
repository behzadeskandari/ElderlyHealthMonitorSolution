using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElderlyHealthMonitor.DTOS.DTO
{
    public class SensorReadingDto
    {
        public Guid DeviceId { get; set; }
        public Guid ElderlyProfileId { get; set; }
        public string SensorType { get; set; } = string.Empty;
        public DateTime TimestampUtc { get; set; }
        public double? Value { get; set; }
        public string? Unit { get; set; }
        public string? PayloadJson { get; set; }
    }
}

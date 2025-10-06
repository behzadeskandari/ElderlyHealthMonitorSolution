using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElderlyHealthMonitor.Edge.Models
{
    public class SensorData
    {
        public string DeviceId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string SensorType { get; set; }  // e.g. "heart_rate", "step_count", "acceleration"
        public double Value { get; set; }
    }
}

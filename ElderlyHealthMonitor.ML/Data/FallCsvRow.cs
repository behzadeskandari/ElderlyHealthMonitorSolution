using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElderlyHealthMonitor.ML.Data
{
    public record FallCsvRow
    {
        public string Timestamp { get; set; } = "";
        public float AccX { get; set; }
        public float AccY { get; set; }
        public float AccZ { get; set; }
        public float GyroX { get; set; }
        public float GyroY { get; set; }
        public float GyroZ { get; set; }
        public float HeartRate { get; set; }
        public bool Label { get; set; } // fall = true
    }
}

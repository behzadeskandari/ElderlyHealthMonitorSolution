using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElderlyHealthMonitor.Domain.ValueObjects
{
    public record HeartRate(int Bpm)
    {
        public bool IsBradycardic => Bpm < 50;
        public bool IsTachycardic => Bpm > 100;
    }
}

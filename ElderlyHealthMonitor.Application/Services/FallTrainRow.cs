using ElderlyHealthMonitor.ML.Trainers;

namespace ElderlyHealthMonitor.Application.Services
{
    internal class FallTrainRow : FallTrainData
    {
        public float[] Features { get; set; }
    }
}
namespace ElderlyHealthMonitor.ML.Pipelines
{
    internal class FallTrainRow
    {
        public float[] Features { get; set; }
        public bool Label { get; set; }
    }
}
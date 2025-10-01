using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Application.DTOs;
using ElderlyHealthMonitor.DTOS.DTO;

namespace ElderlyHealthMonitor.Edge.Preprocessing
{
    public static class FeatureExtractor
    {
        // Build a fixed-length feature vector for fall detection from windowed sensor readings
        // This is a simple example: compute time-domain features of acc magnitude
        public static float[] BuildFeaturesFromWindow(IEnumerable<SensorReadingDto> window)
        {
            var acc = window.Where(w => w.SensorType == "acc" && w.Value.HasValue).Select(w => w.Value!.Value).ToArray();
            if (acc.Length == 0) return new float[64];


            var mean = (float)acc.Average();
            var std = (float)Math.Sqrt(acc.Select(v => (v - mean) * (v - mean)).Average());
            var max = (float)acc.Max();
            var min = (float)acc.Min();


            var features = new float[64];
            features[0] = mean; features[1] = std; features[2] = max; features[3] = min;
            // fill the rest with downsampled magnitudes
            var step = Math.Max(1, acc.Length / (64 - 4));
            int idx = 4;
            for (int i = 0; i < acc.Length && idx < 64; i += step)
            {
                features[idx++] = (float)acc[i];
            }
            return features;
        }
    }
}

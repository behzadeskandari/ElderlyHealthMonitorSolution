using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Application.DTOs;
using ElderlyHealthMonitor.DTOS.DTO;

namespace ElderlyHealthMonitor.Edge.Preprocessing
{
    //public static class FeatureExtractor
    //{
    //    // Build a fixed-length feature vector for fall detection from windowed sensor readings
    //    // This is a simple example: compute time-domain features of acc magnitude
    //    public static float[] BuildFeaturesFromWindow(IEnumerable<SensorReadingDto> window)
    //    {
    //        var acc = window.Where(w => w.SensorType == "acc" && w.Value.HasValue).Select(w => w.Value!.Value).ToArray();
    //        if (acc.Length == 0) return new float[64];


    //        var mean = (float)acc.Average();
    //        var std = (float)Math.Sqrt(acc.Select(v => (v - mean) * (v - mean)).Average());
    //        var max = (float)acc.Max();
    //        var min = (float)acc.Min();


    //        var features = new float[64];
    //        features[0] = mean; features[1] = std; features[2] = max; features[3] = min;
    //        // fill the rest with downsampled magnitudes
    //        var step = Math.Max(1, acc.Length / (64 - 4));
    //        int idx = 4;
    //        for (int i = 0; i < acc.Length && idx < 64; i += step)
    //        {
    //            features[idx++] = (float)acc[i];
    //        }
    //        return features;
    //    }
    //}

    public static class FeatureExtractor
    {
        // Build a fixed-length float[] features vector for a window of SensorReadingDto
        // For accelerometer we expect value to be magnitude or X/Y/Z (here we assume value is magnitude for simplicity).
        public static float[] BuildFeaturesFromWindow(IEnumerable<SensorReadingDto> window)
        {
            var w = window.Where(x => x != null).ToList();
            // Acc magnitude samples (if sensor type 'acc' and value present)
            var acc = w.Where(r => r.SensorType == "acc" && r.Value.HasValue).Select(r => (float)r.Value!.Value).ToArray();
            var hr = w.Where(r => r.SensorType == "hr" && r.Value.HasValue).Select(r => (float)r.Value!.Value).ToArray();

            if (acc.Length == 0)
            {
                // fallback zero features
                return new float[12];
            }

            float mean = acc.Average();
            float std = (float)Math.Sqrt(acc.Select(v => Math.Pow(v - mean, 2)).Average());
            float max = acc.Max();
            float min = acc.Min();
            float energy = acc.Select(v => v * v).Sum();
            float median = acc.OrderBy(v => v).ElementAt(acc.Length / 2);
            float skewness = ComputeSkewness(acc, mean, std);
            float kurt = ComputeKurtosis(acc, mean, std);

            float hrMean = hr.Length > 0 ? hr.Average() : 0f;
            float hrStd = hr.Length > 0 ? (float)Math.Sqrt(hr.Select(v => Math.Pow(v - hrMean, 2)).Average()) : 0f;

            var features = new float[12];
            features[0] = mean;
            features[1] = std;
            features[2] = max;
            features[3] = min;
            features[4] = energy;
            features[5] = median;
            features[6] = skewness;
            features[7] = kurt;
            features[8] = hrMean;
            features[9] = hrStd;
            // fill two slots with downsampled edge values
            features[10] = acc.Length > 0 ? acc.First() : 0f;
            features[11] = acc.Length > 1 ? acc[Math.Max(0, acc.Length - 1)] : 0f;
            return features;
        }

        static float ComputeSkewness(float[] data, float mean, float std)
        {
            if (std == 0) return 0f;
            return data.Select(x => (float)Math.Pow((x - mean) / std, 3)).Average();
        }
        static float ComputeKurtosis(float[] data, float mean, float std)
        {
            if (std == 0) return 0f;
            return data.Select(x => (float)Math.Pow((x - mean) / std, 4)).Average();
        }
    }
}


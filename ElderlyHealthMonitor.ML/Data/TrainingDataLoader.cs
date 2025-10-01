using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElderlyHealthMonitor.ML.Data
{
    public class TrainingDataLoader
    {
        public static IEnumerable<FallCsvRow> LoadFromCsv(string csvPath)
        {
            var lines = System.IO.File.ReadAllLines(csvPath);
            if (lines.Length <= 1) return Enumerable.Empty<FallCsvRow>();
            var header = lines[0].Split(',');
            var rows = new List<FallCsvRow>();
            for (int i = 1; i < lines.Length; i++)
            {
                var cols = lines[i].Split(',');
                if (cols.Length < 9) continue;
                if (!float.TryParse(cols[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var accx)) continue;
                rows.Add(new FallCsvRow
                {
                    Timestamp = cols[0],
                    AccX = accx,
                    AccY = float.Parse(cols[2], CultureInfo.InvariantCulture),
                    AccZ = float.Parse(cols[3], CultureInfo.InvariantCulture),
                    GyroX = float.Parse(cols[4], CultureInfo.InvariantCulture),
                    GyroY = float.Parse(cols[5], CultureInfo.InvariantCulture),
                    GyroZ = float.Parse(cols[6], CultureInfo.InvariantCulture),
                    HeartRate = float.Parse(cols[7], CultureInfo.InvariantCulture),
                    Label = cols[8].Trim() == "1" || cols[8].Trim().ToLower() == "true"
                });
            }
            return rows;
        }


        // Example: Convert consecutive windows (sliding) into training examples.
        // windowSize is number of samples per window (e.g. 50 for 1s@50Hz)
        public static IEnumerable<(float[] Features, bool Label)> BuildWindowedExamples(IEnumerable<FallCsvRow> rows, int windowSize = 32, int step = 16)
        {
            var list = rows.ToList();
            for (int start = 0; start + windowSize <= list.Count; start += step)
            {
                var window = list.Skip(start).Take(windowSize).ToArray();
                // label a window as positive if any sample label==true inside (or majority)
                bool label = window.Any(w => w.Label);
                var features = BuildFeatures(window);
                yield return (features, label);
            }
        }

        public static float[] BuildFeatures(FallCsvRow[] window)
        {
            var mags = window.Select(w => MathF.Sqrt(w.AccX * w.AccX + w.AccY * w.AccY + w.AccZ * w.AccZ)).ToArray();
            var mean = mags.Average();
            var std = MathF.Sqrt(mags.Select(m => (m - mean) * (m - mean)).Average());
            var max = mags.Max();
            var min = mags.Min();
            var hrMean = window.Average(w => w.HeartRate);
            // pack into fixed vector (extendable)
            var fv = new float[10];
            fv[0] = mean;
            fv[1] = std;
            fv[2] = max;
            fv[3] = min;
            fv[4] = hrMean;
            // fill remaining with first few raw mags (downsampled)
            for (int i = 0; i < 5 && i < mags.Length; i++)
                fv[5 + i] = mags[i];
            return fv;
        }
    }

}


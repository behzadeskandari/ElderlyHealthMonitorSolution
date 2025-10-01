using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.ML.Data;
using Microsoft.ML;

namespace ElderlyHealthMonitor.ML.Pipelines
{
    public class HeartRateAnomalyPipeline
    {
        public static void TrainAndSave(string csvPath, string outputModelPath)
        {
            var ml = new MLContext(seed: 0);
            var rows = TrainingDataLoader.LoadFromCsv(csvPath);
            var hrSeries = rows.Select(r => new HRRow { HR = r.HeartRate }).ToList();
            var data = ml.Data.LoadFromEnumerable(hrSeries);

            var pipeline = ml.Transforms.DetectIidSpike(
                outputColumnName: "Prediction",
                inputColumnName: nameof(HRRow.HR),
                confidence: 95,
                pvalueHistoryLength: 30);

            var model = pipeline.Fit(data);
            Directory.CreateDirectory(Path.GetDirectoryName(outputModelPath) ?? ".");
            ml.Model.Save(model, data.Schema, outputModelPath);
            Console.WriteLine($"HR anomaly model saved: {outputModelPath}");
        }

        private class HRRow { public float HR { get; set; } }
    }
}

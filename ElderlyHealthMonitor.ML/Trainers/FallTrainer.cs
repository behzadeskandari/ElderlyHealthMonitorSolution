using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace ElderlyHealthMonitor.ML.Trainers
{
    public class FallTrainData
    {
        [VectorType(10)]
        public float[] Features { get; set; } = new float[10];
        public bool Label { get; set; }
    }

    public class FallPrediction
    {
        [ColumnName("PredictedLabel")] public bool PredictedLabel { get; set; }
        public float Probability { get; set; }
        public float Score { get; set; }
    }

    public static class FallTrainer
    {
        public static void TrainAndSave(string csvPath, string outputModelPath)
        {
            var ml = new MLContext(seed: 0);
            var rows = ElderlyHealthMonitor.ML.Data.TrainingDataLoader.LoadFromCsv(csvPath);
            var examples = ElderlyHealthMonitor.ML.Data.TrainingDataLoader.BuildWindowedExamples(rows, windowSize: 32, step: 16)
                .Select(x => new FallTrainData { Features = x.Features, Label = x.Label }).ToList();

            var data = ml.Data.LoadFromEnumerable(examples);

            var pipeline = ml.Transforms.Concatenate("Features", "Features")
                .Append(ml.Transforms.NormalizeMinMax("Features"))
                .Append(ml.BinaryClassification.Trainers.LbfgsLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));

            var model = pipeline.Fit(data);
            ml.Model.Save(model, data.Schema, outputModelPath);

            Console.WriteLine($"Saved model to: {outputModelPath}");
        }
    }


    public class HrData
    {
        public float HR { get; set; }
    }

    public class HrTrainer
    {
        public static void TrainAndSave(string csvPath, string outputModelPath)
        {
            var ml = new MLContext(seed: 0);
            var rows = ElderlyHealthMonitor.ML.Data.TrainingDataLoader.LoadFromCsv(csvPath);
            // map to HR series
            var hrSeries = rows.Select(r => new HrData { HR = r.HeartRate }).ToList();
            var data = ml.Data.LoadFromEnumerable(hrSeries);

            // DetectIidSpike expects a column; we will fit and save the transformer
            var pipeline = ml.Transforms.DetectIidSpike(
                outputColumnName: "Prediction",
                inputColumnName: nameof(HrData.HR),
                confidence: 95,
                pvalueHistoryLength: 30);

            var model = pipeline.Fit(data);
            ml.Model.Save(model, data.Schema, outputModelPath);
            Console.WriteLine($"Saved HR anomaly model to: {outputModelPath}");
        }
    }
}

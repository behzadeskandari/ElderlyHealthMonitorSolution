using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.ML.Data;
using Microsoft.ML;

namespace ElderlyHealthMonitor.ML.Pipelines
{
    public static class FallDetectionPipeline
    {
        // train and save model given a CSV path and output path
        public static void TrainAndSave(string csvPath, string outputModelPath)
        {
            var ml = new MLContext(seed: 0);

            // Simple CSV loader: we rely on TrainingDataLoader in repo or use quick csv parse
            var raw = TrainingDataLoader.LoadFromCsv(csvPath); // uses FallCsvRow
            var windows = TrainingDataLoader.BuildWindowedExamples(raw, windowSize: 32, step: 16);

            var data = new List<FallTrainRow>();
            foreach (var w in windows)
            {
                var features = w.Features; // already float[]
                data.Add(new FallTrainRow { Features = features, Label = w.Label });
            }

            var dataView = ml.Data.LoadFromEnumerable(data);
            var pipeline = ml.Transforms.NormalizeMinMax("Features")
                .Append(ml.BinaryClassification.Trainers.LbfgsLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));

            var model = pipeline.Fit(dataView);
            Directory.CreateDirectory(Path.GetDirectoryName(outputModelPath) ?? ".");
            ml.Model.Save(model, dataView.Schema, outputModelPath);
            Console.WriteLine($"Fall model saved: {outputModelPath}");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;

namespace ElderlyHealthMonitor.ML.Pipelines
{
    public static class AnomalyDetectionPipeline
    {
        // Basic KMeans cluster model for behavioral patterns (example)
        public static void TrainKMeansAndSave(string featuresCsv, string outputModelPath)
        {
            var ml = new MLContext(seed: 0);
            // Expect a CSV of precomputed daily features where first columns are floats and no label
            var data = ml.Data.LoadFromTextFile<DailyFeature>("./data/daily_features.csv", hasHeader: true, separatorChar: ',');
            var pipeline = ml.Transforms.Concatenate("Features", "F1", "F2", "F3", "F4", "F5")
                .Append(ml.Clustering.Trainers.KMeans("Features", numberOfClusters: 2));
            var model = pipeline.Fit(data);
            Directory.CreateDirectory(Path.GetDirectoryName(outputModelPath) ?? ".");
            ml.Model.Save(model, data.Schema, outputModelPath);
        }

        private class DailyFeature { public float F1; public float F2; public float F3; public float F4; public float F5; }
    }
}

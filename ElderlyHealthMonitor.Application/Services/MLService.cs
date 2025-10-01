using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Application.DTOs;
using ElderlyHealthMonitor.Application.Interfaces;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace ElderlyHealthMonitor.Application.Services
{
    public class MLService : IMLService
    {
        private readonly MLContext _ml;
        private readonly string _modelsFolder;


        public MLService(string modelsFolder = "./models")
        {
            _ml = new MLContext(seed: 0);
            _modelsFolder = modelsFolder;
        }


        // Use SSA anomaly detector on aggregated daily steps/hrs or similar features
        public async Task<AnomalyResult> DetectBehavioralAnomalyAsync(Guid elderlyId, IEnumerable<SensorReadingDto> readings, CancellationToken ct = default)
        {
            // Example: detect spike in hr or change in step-count. We'll use simple IID spike detector on a single numeric stream (e.g. daily activity)
            var values = readings.Where(r => r.Value.HasValue).Select(r => new SimpleFeature { Value = (float)r.Value!.Value });
            var data = _ml.Data.LoadFromEnumerable(values);


            var pipeline = _ml.Transforms.DetectIidSpike(outputColumnName: "Prediction", inputColumnName: nameof(SimpleFeature.Value), pvalueHistoryLength: 30, alarmingThreshold: 95);
            var model = pipeline.Fit(data);


            // Score last sample
            using var engine = _ml.Model.CreateTimeSeriesEngine<SimpleFeature, SpikePrediction>(model);
            var last = values.LastOrDefault();
            if (last == null) return new AnomalyResult(false, 0);
            var pred = engine.Predict(last);
            var isSpike = pred.Prediction[0] == 1;
            return new AnomalyResult(isSpike, pred.Score[0]);
        }


        public async Task<FallResult> DetectFallAsync(IEnumerable<SensorReadingDto> window, CancellationToken ct = default)
        {
            // Supervised approach: load trained model if exists and predict
            var modelPath = Path.Combine(_modelsFolder, "fall_model.zip");
            if (!File.Exists(modelPath))
            {
                // fallback: simple threshold rule on accelerometer magnitude
                var accSamples = window.Where(w => w.SensorType == "acc" && w.PayloadJson != null).ToList();
                // very naive: if any value > threshold => fall
                foreach (var s in accSamples)
                {
                    if (s.Value.HasValue && Math.Abs(s.Value.Value) > 2.5) // g-force
                        return new FallResult(true, 0.6);
                }
                return new FallResult(false, 0.0);
            }


            var loaded = _ml.Model.Load(modelPath, out var schema);
            var engine = _ml.Model.CreatePredictionEngine<FallInput, FallPrediction>(loaded);


            var features = FeatureExtractor.BuildFeaturesFromWindow(window);
            var input = new FallInput { Features = features };
            var p = engine.Predict(input);
            return new FallResult(p.PredictedLabel, p.Probability);
        }


        private class SimpleFeature { public float Value { get; set; } }
        private class SpikePrediction { [VectorType(3)] public double[] Prediction { get; set; } = new double[3]; public double[] Score { get; set; } = new double[3]; }


        // fall model input classes
        private class FallInput { [VectorType(64)] public float[] Features { get; set; } = new float[64]; }
        private class FallPrediction { public bool PredictedLabel { get; set; } public float Probability { get; set; } public float Score { get; set; } }
    }
}

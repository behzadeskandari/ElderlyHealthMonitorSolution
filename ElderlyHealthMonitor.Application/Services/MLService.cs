using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Application.DTOs;
using ElderlyHealthMonitor.Application.Interfaces;
using ElderlyHealthMonitor.Application.Records;
using ElderlyHealthMonitor.DTOS.DTO;
using ElderlyHealthMonitor.Edge.Preprocessing;
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

        Task<AnomalyResult> IMLService.DetectBehavioralAnomalyAsync(Guid elderlyId, IEnumerable<SensorReadingDto> readings, CancellationToken ct)
        {
            // prepare numeric stream (example: use Value field)
            var values = readings.Where(r => r.Value.HasValue).Select(r => new SingleValue { Value = (float)r.Value!.Value }).ToList();
            if (!values.Any()) return Task.FromResult(new AnomalyResult(false, 0.0));

            var data = _ml.Data.LoadFromEnumerable(values);

            // create spike detector transform
            var pipeline = _ml.Transforms.DetectIidSpike(
                outputColumnName: "Prediction",
                inputColumnName: nameof(SingleValue.Value),
                pvalueHistoryLength: 30, // tune
                trainingWindowSize: 30,
                confidence: 95);

            var transformer = pipeline.Fit(data);

            // Use TimeSeries prediction engine if available
            try
            {
                var engine = _ml.Model.CreateTimeSeriesEngine<SingleValue, SpikePrediction>(transformer);
                var last = values.Last();
                var pred = engine.Predict(last);
                var isSpike = pred.Prediction[0] == 1;
                return Task.FromResult(new AnomalyResult(isSpike, pred.Score[0]));
            }
            catch (MissingMethodException)
            {
                // Fallback: compute last row transform and inspect output column
                var transformed = transformer.Transform(data);
                var preds = _ml.Data.CreateEnumerable<SpikePrediction>(transformed, reuseRowObject: false).ToList();
                var lastPred = preds.LastOrDefault();
                if (lastPred == null) return Task.FromResult(new AnomalyResult(false, 0));
                var isSpike = lastPred.Prediction[0] == 1;
                return Task.FromResult(new AnomalyResult(isSpike, lastPred.Score[0]));
            }
        }

        Task<FallResult> IMLService.DetectFallAsync(IEnumerable<SensorReadingDto> window, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        private class SimpleFeature { public float Value { get; set; } }
        private class SpikePrediction { [VectorType(3)] public double[] Prediction { get; set; } = new double[3]; public double[] Score { get; set; } = new double[3]; }

        private class SingleValue { public float Value { get; set; } }
        // fall model input classes
        private class FallInput { [VectorType(64)] public float[] Features { get; set; } = new float[64]; }
        private class FallPrediction { public bool PredictedLabel { get; set; } 
        public float Probability { get; set; } public float Score { get; set; } }
    }
}

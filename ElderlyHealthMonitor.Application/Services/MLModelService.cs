using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Application.Interfaces;
using ElderlyHealthMonitor.Application.Records;
using ElderlyHealthMonitor.DTOS.DTO;
using Microsoft.ML;

namespace ElderlyHealthMonitor.Application.Services
{
    public class MLModelService : IMLService
    {
        private readonly MLContext _ml;
        private ITransformer? _fallModel;
        private ITransformer? _hrModel;
        private PredictionEngine<ElderlyHealthMonitor.ML.Trainers.FallTrainData, ElderlyHealthMonitor.ML.Trainers.FallPrediction>? _fallEngine;

        private readonly string _modelsFolder;

        public MLModelService(string modelsFolder = "./Models")
        {
            _ml = new MLContext(seed: 0);
            _modelsFolder = modelsFolder;
            LoadModels();
        }

        private void LoadModels()
        {
            var fallPath = System.IO.Path.Combine(_modelsFolder, "fall_model.zip");
            var hrPath = System.IO.Path.Combine(_modelsFolder, "hr_model.zip");

            if (System.IO.File.Exists(fallPath))
            {
                _fallModel = _ml.Model.Load(fallPath, out var schema);
                _fallEngine = _ml.Model.CreatePredictionEngine<ElderlyHealthMonitor.ML.Trainers.FallTrainData, ElderlyHealthMonitor.ML.Trainers.FallPrediction>(_fallModel);
            }

            if (System.IO.File.Exists(hrPath))
            {
                _hrModel = _ml.Model.Load(hrPath, out var schema);
            }
        }

        // Detect behavioral anomaly: use simple hr spike detector as approximate
        public async Task<AnomalyResult> DetectBehavioralAnomalyAsync(Guid elderlyId, IEnumerable<SensorReadingDto> readings, CancellationToken ct = default)
        {
            // For MVP: check HR spike via HR model (if exists) or simple threshold
            var lastHr = readings.Where(r => r.SensorType == "hr" && r.Value.HasValue).Select(r => (float)r.Value!.Value).LastOrDefault();
            if (lastHr == 0) return new AnomalyResult(false, 0);

            if (_hrModel != null)
            {
                var data = new List<object> { new { HR = lastHr } };
                var dv = _ml.Data.LoadFromEnumerable(data);
                var transformed = _hrModel.Transform(dv);
                var preds = _ml.Data.CreateEnumerable<dynamic>(transformed, reuseRowObject: false).ToList();
                // `DetectIidSpike` outputs vector columns; parse generically
                // fallback: treat lastHr > 120 as anomaly
            }

            if (lastHr > 120 || lastHr < 40) return new AnomalyResult(true, Math.Abs(lastHr - 75));
            return new AnomalyResult(false, 0);
        }

        public async Task<FallResult> DetectFallAsync(IEnumerable<SensorReadingDto> window, CancellationToken ct = default)
        {
            // Build features with same extractor used in training
            var csvWindow = window.Select(w => new ElderlyHealthMonitor.ML.Data.FallCsvRow
            {
                AccX = (float)(w.Value ?? 0),
                AccY = 0,
                AccZ = 0,
                GyroX = 0,
                GyroY = 0,
                GyroZ = 0,
                HeartRate = 0,
                Label = false
            }).ToArray();

            // But better: call FeatureExtractor.BuildFeaturesFromWindow if you implemented one; here we reuse training loader
            var features = ElderlyHealthMonitor.ML.Data.TrainingDataLoader.BuildFeatures(csvWindow);

            var input = new ElderlyHealthMonitor.ML.Trainers.FallTrainData { Features = features };

            if (_fallEngine != null)
            {
                var p = _fallEngine.Predict(input);
                return new FallResult(p.PredictedLabel, p.Probability);
            }

            // fallback: simple magnitude threshold on acc (if window contains 'acc' sensor type and values)
            var accVals = window.Where(w => w.SensorType == "acc" && w.Value.HasValue).Select(w => Math.Abs(w.Value!.Value));
            if (accVals.Any(v => v > 2.5)) return new FallResult(true, 0.6);
            return new FallResult(false, 0.0);
        }
    }
}

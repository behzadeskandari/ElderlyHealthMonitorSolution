using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Application.Interfaces;
using ElderlyHealthMonitor.Application.Records;
using ElderlyHealthMonitor.DTOS.DTO;
using ElderlyHealthMonitor.Edge.Preprocessing;
using ElderlyHealthMonitor.ML.Trainers;
using Microsoft.ML;

namespace ElderlyHealthMonitor.Application.Services
{
    public class MLModelService : IMLService
    {
        private readonly MLContext _ml;
        private ITransformer? _fallModel;
        private PredictionEngine<FallTrainRow, FallPrediction>? _fallEngine;
        private ITransformer? _hrModel;
        private ITransformer? _anomalyModel;
        private readonly string _modelsFolder;


        public MLModelService(string modelsFolder = null)
        {
            _ml = new MLContext(seed: 0);
            _modelsFolder = modelsFolder ?? Path.Combine(AppContext.BaseDirectory, "Models");
            LoadModels();
        }

        private void LoadModels()
        {
            try
            {
                var fallPath = Path.Combine(_modelsFolder, "fall_model.csv");
                if (File.Exists(fallPath))
                {
                    _fallModel = _ml.Model.Load(fallPath, out var schema);
                    _fallEngine = _ml.Model.CreatePredictionEngine<FallTrainRow, FallPrediction>(_fallModel);
                }

                var hrPath = Path.Combine(_modelsFolder, "hr_model.zip");
                if (File.Exists(hrPath))
                {
                    _hrModel = _ml.Model.Load(hrPath, out var schema);
                }

                var anomalyPath = Path.Combine(_modelsFolder, "anomaly_model.zip");
                if (File.Exists(anomalyPath))
                {
                    _anomalyModel = _ml.Model.Load(anomalyPath, out var schema);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading ML models: " + ex.Message);
            }
        }
        // Detect behavioral anomaly: use simple hr spike detector as approximate
        //public async Task<AnomalyResult> DetectBehavioralAnomalyAsync(Guid elderlyId, IEnumerable<SensorReadingDto> readings, CancellationToken ct = default)
        //{
        //    // For MVP: check HR spike via HR model (if exists) or simple threshold
        //    var lastHr = readings.Where(r => r.SensorType == "hr" && r.Value.HasValue).Select(r => (float)r.Value!.Value).LastOrDefault();
        //    if (lastHr == 0) return new AnomalyResult(false, 0);

        //    if (_hrModel != null)
        //    {
        //        var data = new List<object> { new { HR = lastHr } };
        //        var dv = _ml.Data.LoadFromEnumerable(data);
        //        var transformed = _hrModel.Transform(dv);
        //        var preds = _ml.Data.CreateEnumerable<dynamic>(transformed, reuseRowObject: false).ToList();
        //        // `DetectIidSpike` outputs vector columns; parse generically
        //        // fallback: treat lastHr > 120 as anomaly
        //    }

        //    if (lastHr > 120 || lastHr < 40) return new AnomalyResult(true, Math.Abs(lastHr - 75));
        //    return new AnomalyResult(false, 0);
        //}

       
        public Task<AnomalyResult> DetectBehavioralAnomalyAsync(Guid elderlyId, IEnumerable<SensorReadingDto> readings, CancellationToken ct = default)
        {
            // For MVP, check HR model then threshold fallback
            var hrVals = readings.Where(r => r.SensorType == "hr" && r.Value.HasValue).Select(r => (float)r.Value!.Value).ToArray();
            if (hrVals.Length == 0) return Task.FromResult(new AnomalyResult(false, 0));

            var last = hrVals.Last();

            if (_hrModel != null)
            {
                var dv = _ml.Data.LoadFromEnumerable(new[] { new { HR = last } });
                var transformed = _hrModel.Transform(dv);
                var rows = _ml.Data.CreateEnumerable<GenericSpikePrediction>(transformed, reuseRowObject: false).ToList();
                if (rows.Count > 0)
                {
                    var pred = rows.Last();
                    var isSpike = pred.Prediction[0] == 1;
                    return Task.FromResult(new AnomalyResult(isSpike, pred.Score[0]));
                }
            }

            // fallback simple threshold
            if (last > 120 || last < 40) return Task.FromResult(new AnomalyResult(true, Math.Abs(last - 75)));
            return Task.FromResult(new AnomalyResult(false, 0));
        }

        public Task<FallResult> DetectFallAsync(IEnumerable<SensorReadingDto> window, CancellationToken ct = default)
        {
            // build features
            var feats = FeatureExtractor.BuildFeaturesFromWindow(window);
            var input = new FallTrainRow { Features = feats };

            if (_fallEngine != null)
            {
                var p = _fallEngine.Predict(input);
                return Task.FromResult(new FallResult(p.PredictedLabel, p.Probability));
            }

            // fallback threshold on magnitude
            var accVals = window.Where(r => r.SensorType == "acc" && r.Value.HasValue).Select(r => Math.Abs(r.Value!.Value));
            if (accVals.Any(v => v > 2.5)) return Task.FromResult(new FallResult(true, 0.6));
            return Task.FromResult(new FallResult(false, 0.0));
        }

        private class GenericSpikePrediction { public double[] Prediction { get; set; } = new double[3]; public double[] Score { get; set; } = new double[3]; }

    }
}

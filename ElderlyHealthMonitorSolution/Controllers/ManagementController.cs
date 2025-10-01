using ElderlyHealthMonitor.ML.Pipelines;
using Microsoft.AspNetCore.Mvc;

namespace ElderlyHealthMonitorSolution.API.Controllers
{
    [ApiController]
    [Route("api/manage")]
    public class ManagementController : ControllerBase
    {
        // Dev-only: trigger training using CSV placed under ML project data
        [HttpPost("train")]
        public IActionResult TrainModels([FromQuery] string csvPath = "data/synthetic_fall_dataset.csv")
        {
            var mlProjectPath = Path.Combine(AppContext.BaseDirectory, "..", "ElderlyHealthMonitor.ML");
            // If running from solution root, you might need to point to ML project location. For runtime use absolute paths.
            // We'll call the pipelines directly:
            var modelsDir = Path.Combine(AppContext.BaseDirectory, "Models");
            Directory.CreateDirectory(modelsDir);
            FallDetectionPipeline.TrainAndSave(csvPath, Path.Combine(modelsDir, "fall_model.zip"));
            HeartRateAnomalyPipeline.TrainAndSave(csvPath, Path.Combine(modelsDir, "hr_model.zip"));
            // Anomaly trainer optional
            return Ok(new { status = "trained", modelsDir });
        }
    }
}

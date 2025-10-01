using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.ML.Pipelines;

namespace ElderlyHealthMonitor.ML.Trainers
{
    public static class HrAnomalyTrainer
    {
        public static void Run(string csvPath, string modelOut)
        {
            HeartRateAnomalyPipeline.TrainAndSave(csvPath, modelOut);
        }
    }
}

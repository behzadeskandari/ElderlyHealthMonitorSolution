using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.ML.Pipelines;

namespace ElderlyHealthMonitor.ML.Trainers
{
    public static class AnomalyTrainer
    {
        public static void RunKMeans(string featuresCsv, string modelOut)
        {
            AnomalyDetectionPipeline.TrainKMeansAndSave(featuresCsv, modelOut);
        }
    }
}

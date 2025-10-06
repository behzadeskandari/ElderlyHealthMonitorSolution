using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElderlyHealthMonitorSolution.ML.Test
{
    public  class EnumerableModelTest
    {
        public static void Execute()
        {
            MLContext mLContext = new MLContext();

            List<InputModel> data = new List<InputModel>()
            {
                new InputModel { YearOfExperience = 1 , Salary = 39000 },
                new InputModel { YearOfExperience = 1.3F , Salary = 46200 },
                new InputModel { YearOfExperience = 1.5F , Salary = 23700 },
                new InputModel { YearOfExperience = 2 , Salary = 43500 },
                new InputModel { YearOfExperience = 2.2F , Salary = 40000 },
                new InputModel { YearOfExperience = 2.9F , Salary = 56000 },
                new InputModel { YearOfExperience = 3 , Salary = 60000 },
                new InputModel { YearOfExperience = 3.2F , Salary = 54000 },
                new InputModel { YearOfExperience = 3.3F , Salary = 64000 },
                new InputModel { YearOfExperience = 3.7F , Salary = 57000 },
                new InputModel { YearOfExperience = 3.9F , Salary = 63000 },
                new InputModel { YearOfExperience = 4 , Salary = 55000 },
                new InputModel { YearOfExperience = 4 , Salary = 58000 },
                new InputModel { YearOfExperience = 4.1F , Salary = 57000 },
                new InputModel { YearOfExperience = 4.5F , Salary = 61000 },
                new InputModel { YearOfExperience = 4.9F , Salary = 68000 },
                new InputModel { YearOfExperience = 5.3F , Salary = 83000 },
                new InputModel { YearOfExperience = 5.9F , Salary = 82000 },
                new InputModel { YearOfExperience = 6 , Salary = 94000 },
                new InputModel { YearOfExperience = 6.8F , Salary = 91000 },
                new InputModel { YearOfExperience = 7.1F , Salary = 98000 },
                new InputModel { YearOfExperience = 7.9F , Salary = 101000 },
                new InputModel { YearOfExperience = 8.2F , Salary = 114000 },
                new InputModel { YearOfExperience = 8.9F , Salary = 109000 },
            };


            IDataView trainingData = mLContext.Data.LoadFromEnumerable<InputModel>(data);

            var estimator = mLContext.Transforms.Concatenate("Features", new[] { "YearOfExperience" });


            var pipeline = estimator.Append(mLContext.Regression.Trainers.Sdca(labelColumnName: "Salary" , maximumNumberOfIterations: 100));



            var model = pipeline.Fit(trainingData);
            var perdictionEngine = mLContext.Model.CreatePredictionEngine<InputModel,ResultModel>(model);


            var experience = new InputModel { YearOfExperience = 5 };
            
            ResultModel result = perdictionEngine.Predict(experience);

        }
    }
}

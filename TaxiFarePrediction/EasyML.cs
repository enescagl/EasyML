using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.ML;
using Microsoft.ML.Core.Data;
using Microsoft.ML.Runtime.Data;

namespace TaxiFarePrediction
{
    public class EasyML
    {
        #region Properties
        public string PathToTrainingData { get; set; }
        public string PathToTestingData { get; set; }
        public string SaveModelTo { get; set; }
        public string LoadModelFrom { get; set; }
        // public List<string> PropertyNames { get; set; }
        private MLContext MLContext { get; set; }
        private DataExtraction dataExtractor;
        #endregion

        #region Constructors
        public EasyML(string modelName, MLContext mlContext, string separator = ",")
        {
            this.PathToTrainingData = Path.Combine(Environment.CurrentDirectory, "Data", "taxi-fare-train.csv");
            this.PathToTestingData = Path.Combine(Environment.CurrentDirectory, "Data", "taxi-fare-test.csv");
            this.SaveModelTo = Path.Combine(Environment.CurrentDirectory, "Data", $"{modelName}_{DateTime.Now.ToString("yyyy-mm-dd hh-mm")}.zip");
            this.MLContext = mlContext;
            dataExtractor = new DataExtraction(MLContext, PathToTrainingData);
            dataExtractor.ConstructDataStructure(separator);
        }

        public EasyML(string modelName, string trainingDataPath, string testingDataPath, string saveModelPath, MLContext mlContext, string separator = ",")
        {
            this.PathToTrainingData = trainingDataPath;
            this.PathToTestingData = testingDataPath;
            this.SaveModelTo = Path.Combine(modelName, $"{modelName}_{DateTime.Now.ToString("yyyy-mm-dd hh-mm")}.zip");
            this.MLContext = mlContext;
            dataExtractor = new DataExtraction(MLContext, PathToTrainingData);
            dataExtractor.ConstructDataStructure(separator);
        }
        #endregion

        #region DataModelCreationProcess

        #endregion

        #region ModelCreationProcess
        public ITransformer CreateFittedModel(IEstimator<ITransformer> pipeline)
        {
            IDataView dataView = dataExtractor.textLoader.Read(PathToTrainingData);

            ITransformer model = pipeline.Fit(dataView);
            return model;
        }

        public void SaveModelAsFile(ITransformer model)
        {
            using(var fileStream = new FileStream(SaveModelTo, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                MLContext.Model.Save(model, fileStream);
            }
            // return $"Model saved to {SaveModelTo}";
        }
        #endregion

        #region TestingAndEvaluationProcess
        public TScore TestSinglePrediction<TTest, TScore>(TTest TestDataBasedOnModel, string loadModelFrom)
        where TTest : class
        where TScore : class, new()
        {
            ITransformer loadedModel;
            using(var stream = new FileStream(loadModelFrom, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                loadedModel = MLContext.Model.Load(stream);
            }
            var predictionFunction = loadedModel.MakePredictionFunction<TTest, TScore>(MLContext);
            var testPrediction = predictionFunction.Predict(TestDataBasedOnModel);
            return testPrediction;
        }
        public TScore TestSinglePrediction<TTest, TScore>(TTest TestDataBasedOnModel, ITransformer model)
        where TTest : class
        where TScore : class, new()
        {
            var predictionFunction = model.MakePredictionFunction<TTest, TScore>(MLContext);
            var testPrediction = predictionFunction.Predict(TestDataBasedOnModel);
            return testPrediction;
        }

        public void EvaluateModel(ITransformer model, MLContext Context)
        {
            IDataView dataView = dataExtractor.textLoader.Read(PathToTestingData);

            var predictions = model.Transform(dataView);

            // var metrics = ;
        }
        #endregion
    }
}
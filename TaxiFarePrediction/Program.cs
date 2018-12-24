using System;
using Microsoft.ML;
using Microsoft.ML.Core.Data;

namespace TaxiFarePrediction
{
    class Program
    {
        static void Main(string[] args)
        {
            MLContext mainContext = new MLContext();
            // RegressionContext regressionContext = new RegressionContext(mainContext);

            EasyML MLConstruction = new EasyML("RegressionModel", mainContext);

            IEstimator<ITransformer> pipeline = mainContext.Transforms.CopyColumns("FareAmount", "Label")
                .Append(mainContext.Transforms.Categorical.OneHotEncoding("VendorId"))
                .Append(mainContext.Transforms.Categorical.OneHotEncoding("PaymentType"))
                .Append(mainContext.Transforms.Concatenate("Features", "VendorId", "RateCode", "PassengerCount", "TripTimeInSecs", "TripDistance", "PaymentType"))
                .Append(mainContext.Regression.Trainers.FastTree());

            var model = MLConstruction.CreateFittedModel(pipeline);
            // MLConstruction.SaveModelAsFile(model);
            TaxiTrip TestInstance = new TaxiTrip()
            {
                VendorId = "VTS",
                RateCode = 1,
                PassengerCount = 1,
                TripTimeInSecs = 1140,
                TripDistance = 3.75f,
                PaymentType = "CRD",
                FareAmount = 0.0f // To predict. Actual/Observed = 15.5
            };
            var testScore = MLConstruction.TestSinglePrediction<TaxiTrip, TaxiTripFarePrediction>(TestInstance, model);

            Console.WriteLine($"{testScore.FareAmount}");

        }

    }
}
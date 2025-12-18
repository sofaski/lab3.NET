using Microsoft.ML;
using Microsoft.ML.Data;
using CarValueML.Models;

namespace CarValueML.Services
{
    public class CarMLService
    {
        private readonly MLContext _mlContext;

        // nullable warnings FIX
        private ITransformer _model = null!;
        private PredictionEngine<CarData, CarPrediction> _predictionEngine = null!;

        private const string DataPath = "Data/car_extended.csv";

        public CarMLService()
        {
            _mlContext = new MLContext(seed: 1);
            TrainModel();
        }

        private void TrainModel()
        {
            //Load data
            var dataView = _mlContext.Data.LoadFromTextFile<CarData>(
                path: DataPath,
                hasHeader: false,
                separatorChar: ',');

            //ML pipeline
            var pipeline =
                _mlContext.Transforms.Conversion.MapValueToKey(
                        outputColumnName: "Label",
                        inputColumnName: nameof(CarData.Class))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(nameof(CarData.Buying)))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(nameof(CarData.Maint)))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(nameof(CarData.Doors)))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(nameof(CarData.Persons)))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(nameof(CarData.LugBoot)))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(nameof(CarData.Safety)))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(nameof(CarData.EstimatedPriceCategory)))
                .Append(_mlContext.Transforms.Concatenate(
                    "Features",
                    nameof(CarData.Buying),
                    nameof(CarData.Maint),
                    nameof(CarData.Doors),
                    nameof(CarData.Persons),
                    nameof(CarData.LugBoot),
                    nameof(CarData.Safety),
                    nameof(CarData.EstimatedPriceCategory)))
                .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            //Train
            _model = pipeline.Fit(dataView);

            //Prediction engine
            _predictionEngine =
                _mlContext.Model.CreatePredictionEngine<CarData, CarPrediction>(_model);
        }

        public CarPredictionModel Predict(CarInputModel input)
        {
            var prediction = _predictionEngine.Predict(new CarData
            {
                Buying = input.Buying,
                Maint = input.Maint,
                Doors = input.Doors,
                Persons = input.Persons,
                LugBoot = input.LugBoot,
                Safety = input.Safety,
                EstimatedPriceCategory = input.EstimatedPriceCategory,
                Class = "" // dummy, not used in prediction
            });

            return new CarPredictionModel
            {
                PredictedClass = prediction.PredictedLabel,
                Score = prediction.Score
            };
        }
    }

    //Internal ML classes

    public class CarData
    {
        [LoadColumn(0)] public string Buying { get; set; } = string.Empty;
        [LoadColumn(1)] public string Maint { get; set; } = string.Empty;
        [LoadColumn(2)] public string Doors { get; set; } = string.Empty;
        [LoadColumn(3)] public string Persons { get; set; } = string.Empty;
        [LoadColumn(4)] public string LugBoot { get; set; } = string.Empty;
        [LoadColumn(5)] public string Safety { get; set; } = string.Empty;
        [LoadColumn(6)] public string EstimatedPriceCategory { get; set; } = string.Empty;
        [LoadColumn(7)] public string Class { get; set; } = string.Empty;
    }

    public class CarPrediction
    {
        [ColumnName("PredictedLabel")]
        public string PredictedLabel { get; set; } = string.Empty;

        public float[] Score { get; set; } = Array.Empty<float>();
    }
}

namespace CarValueML.Models;

public class CarPredictionModel
{
    public string PredictedClass { get; set; } = "";
    public float[]? Score { get; set; }
}

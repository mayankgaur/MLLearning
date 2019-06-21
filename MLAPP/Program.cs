using Microsoft.ML;

namespace MLAPP
{
    class Program
    {
        static MLContext mlContext = new MLContext();
        static void Main(string[] args)
        {
            SentimentPredictor.SentimentPrediction(mlContext);
        }
    }
}

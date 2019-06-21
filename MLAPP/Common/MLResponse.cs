namespace MLAPP.Common
{
    public class MLResponse
    {
        public string SentimentText { get; set; }
        public string Prediction { get; set; }
        public float Probability { get; set; }
    }
}

using Microsoft.ML.Data;
using MongoDB.Bson.Serialization.Attributes;

namespace MLAPP.Common
{
    [BsonIgnoreExtraElements]
    public class SentimentData
    {
        [LoadColumn(0)]
        [BsonElement("text")]
        public string SentimentText;

        [LoadColumn(1), ColumnName("Label")]
        [BsonElement("sentiment")]
        public bool Sentiment;
    }
    public class SentimentPrediction : SentimentData
    {

        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        public float Probability { get; set; }

        public float Score { get; set; }
    }
}

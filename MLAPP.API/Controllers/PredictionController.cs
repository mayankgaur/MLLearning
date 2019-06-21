using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;

namespace MLAPP.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PredictionController : ControllerBase
    {

        /// <summary>
        /// Get all Sentiments
        /// </summary>
        /// <param name="Sentiment"></param>
        /// <returns></returns>
        [HttpGet("{Sentiment}")]      
        [ActionName("GetPrediction")]
        public ActionResult Get(string Sentiment)
        {
            MLContext mlContext = new MLContext();

            return Ok(new { Message = SentimentPredictor.TestSinglePrediction(mlContext, Sentiment) });

        }
    }
}
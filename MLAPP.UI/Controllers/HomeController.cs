using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using MLAPP.Common;
using MLAPP.UI.Models;
using System.Diagnostics;

namespace MLAPP.UI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(MLResponse mLResponse)
        {
            return View(mLResponse);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        /// <summary>
        /// Get all Sentiments
        /// </summary>
        /// <param name="Sentiment"></param>
        /// <returns></returns>
        [HttpPost]       
        public IActionResult GetPrediction(string Sentiment)
        {
            MLContext mlContext = new MLContext();
            MLResponse predictionDetail =   SentimentPredictor.TestSinglePrediction(mlContext, Sentiment);

            return RedirectToAction("Index","Home",predictionDetail);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

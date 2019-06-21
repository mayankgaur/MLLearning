using Microsoft.ML;
using Microsoft.ML.Data;
using MLAPP.Common;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using static Microsoft.ML.DataOperationsCatalog;
using Console = Colorful.Console;
namespace MLAPP
{
    public class SentimentPredictor
    {
        #region Private Variable
        private static string mongoURI = "mongodb://localhost:27017";
        private static readonly MongoClient client = new MongoClient(mongoURI);
        private delegate void WriteToScreen(int n);
        private static readonly string BaseModelsRelativePath = @"../../../MLModels";
        private static readonly string ModelRelativePath = $"{BaseModelsRelativePath}/SentimentModel.zip";
        private static string ModelPath = GetAbsolutePath(ModelRelativePath);
        #endregion Private Variable
        public static void SentimentPrediction(MLContext mlContext)
        {
            // Create ML.NET context/local environment - allows you to add steps in order to keep everything together 
            // as you discover the ML.NET trainers and transforms 
            // <SnippetCreateMLContext>


            // </SnippetCreateMLContext>
            Color color = Color.FromArgb(130, 150, 115);
            Console.WriteLine("=============== Reading DB Starts ===============", color);
            // <SnippetCallLoadData>
            TrainTestData splitDataView = LoadData(mlContext);
            // </SnippetCallLoadData>


            // <SnippetCallBuildAndTrainModel>         
            ITransformer model = BuildAndTrainModel(mlContext, splitDataView.TrainSet);
            // </SnippetCallBuildAndTrainModel>

            // <SnippetCallEvaluate>
            Evaluate(mlContext, model, splitDataView.TestSet);
            // </SnippetCallEvaluate>

            // <SnippetCallUseModelWithSingleItem>
            //UseModelWithSingleItem(mlContext, model);
            // </SnippetCallUseModelWithSingleItem>

            // <SnippetCallUseModelWithBatchItems>
            // UseModelWithBatchItems(mlContext, model);
            // </SnippetCallUseModelWithBatchItems>

            Console.WriteLine();
            Console.WriteLine("=============== End of process ===============");
            Console.ReadLine();
        }

        public static TrainTestData LoadData(MLContext mlContext)
        {
            // Note that this case, loading your training data from a file, 
            // is the easiest way to get started, but ML.NET also allows you 
            // to load data from databases or in-memory collections.
            // <SnippetLoadData>
            Color color = Color.FromArgb(130, 150, 115);
            Console.WriteLine("=============== DB Opening ===============", color);
            var db = client.GetDatabase("yelp");
            var collection = db.GetCollection<SentimentData>("review_train");
            var documents = collection.Find<SentimentData>(new BsonDocument()).ToEnumerable();
            Console.WriteLine("=============== Data Collected ===============", color);
            //STEP 2: Read data from MongoDB using LoadFromEnumerable and return dataview.
            // IDataView trainingData = mlContext.Data.LoadFromTextFile<SentimentData>(_dataPath, hasHeader: false);
            IDataView trainingData = mlContext.Data.LoadFromEnumerable<SentimentData>(documents);
            trainingData = mlContext.Data.Cache(trainingData);
            Console.WriteLine("=============== Data Loaded to Training Data ===============", color);
            // </SnippetLoadData>

            // You need both a training dataset to train the model and a test dataset to evaluate the model.
            // Split the loaded dataset into train and test datasets
            // Specify test dataset percentage with the `testFraction`parameter
            // <SnippetSplitData>
            TrainTestData splitDataView = mlContext.Data.TrainTestSplit(trainingData, testFraction: 0.2);
            // </SnippetSplitData>

            // <SnippetReturnSplitData>        
            return splitDataView;
            // </SnippetReturnSplitData>           
        }

        public static ITransformer BuildAndTrainModel(MLContext mlContext, IDataView splitTrainSet)
        {
            // Create a flexible pipeline (composed by a chain of estimators) for creating/training the model.
            // This is used to format and clean the data.  
            // Convert the text column to numeric vectors (Features column) 
            // <SnippetFeaturizeText>
            var estimator = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.SentimentText))
            //</SnippetFeaturizeText>
            // append the machine learning task to the estimator
            // <SnippetAddTrainer> 
            .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));
            // </SnippetAddTrainer>

            // Create and train the model based on the dataset that has been loaded, transformed.
            // <SnippetTrainModel>
            Console.WriteLine("=============== Create and Train the Model ===============");
            var model = estimator.Fit(splitTrainSet);
            Console.WriteLine("=============== End of training ===============");
            Console.WriteLine();
            // </SnippetTrainModel>

            // Returns the model we trained to use for evaluation.
            // <SnippetReturnModel>
            return model;
            // </SnippetReturnModel>
        }


        public static void Evaluate(MLContext mlContext, ITransformer model, IDataView splitTestSet)
        {
            // Evaluate the model and show accuracy stats
            Color color = Color.FromArgb(130, 150, 115);
            //Take the data in, make transformations, output the data. 
            // <SnippetTransformData>
            Console.WriteLine("=============== Evaluating Model accuracy with Test data===============");
            IDataView predictions = model.Transform(splitTestSet);
            var preViewTransformedData = predictions.Preview(maxRows: 20);
            Console.WriteLine("=============== Preview Transform Data Starts ===============", color);

            ConsoleHelper.ProgressTotal = preViewTransformedData.RowView.Length;
            int i = 1;
            foreach (var row in preViewTransformedData.RowView)
            {

                var ColumnCollection = row.Values;
                string lineToPrint = "Row--> ";

                foreach (KeyValuePair<string, object> column in ColumnCollection)
                {
                    lineToPrint += $"| {column.Key}:{column.Value}";
                }
                WriteToScreen nc1 = new WriteToScreen(Progress);
                nc1(i);
                i++;
                Console.WriteLine(lineToPrint + "\n", color);
            }

            // </SnippetTransformData>

            // BinaryClassificationContext.Evaluate returns a BinaryClassificationEvaluator.CalibratedResult
            // that contains the computed overall metrics.
            // <SnippetEvaluate>
            CalibratedBinaryClassificationMetrics metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");

            // </SnippetEvaluate>
            mlContext.Model.Save(model, splitTestSet.Schema, ModelPath);
            // The Accuracy metric gets the accuracy of a model, which is the proportion 
            // of correct predictions in the test set.

            // The AreaUnderROCCurve metric is equal to the probability that the algorithm ranks
            // a randomly chosen positive instance higher than a randomly chosen negative one
            // (assuming 'positive' ranks higher than 'negative').

            // The F1Score metric gets the model's F1 score.
            // The F1 score is the harmonic mean of precision and recall:
            //  2 * precision * recall / (precision + recall).

            // <SnippetDisplayMetrics>
            Console.WriteLine();
            Console.WriteLine("Model quality metrics evaluation");
            Console.WriteLine("--------------------------------");
            Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"Auc: {metrics.AreaUnderRocCurve:P2}");
            Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
            Console.WriteLine("=============== End of model evaluation ===============");
            //</SnippetDisplayMetrics>

        }

        public static void UseModelWithSingleItem(MLContext mlContext, ITransformer model)
        {
            // <SnippetCreatePredictionEngine1>
            PredictionEngine<SentimentData, SentimentPrediction> predictionFunction = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
            // </SnippetCreatePredictionEngine1>

            // <SnippetCreateTestIssue1>

            Console.Write("Please Enter your Thoughts: ");
            SentimentData sampleStatement = new SentimentData
            {
                SentimentText = Console.ReadLine()
            };
            // </SnippetCreateTestIssue1>

            // <SnippetPredict>
            var resultPrediction = predictionFunction.Predict(sampleStatement);
            // </SnippetPredict>
            // <SnippetOutputPrediction>
            Console.WriteLine();
            Console.WriteLine("=============== Prediction Test of model with a single sample and test dataset ===============");

            Console.WriteLine();
            Console.WriteLine($"Sentiment: {resultPrediction.SentimentText} | Prediction: {(resultPrediction.Prediction)} | Probability: {resultPrediction.Probability} ");

            Console.WriteLine("=============== End of Predictions ===============");

            //Console.Write("Do You want to Continue(Y/N): ");
            //if (Console.ReadLine() == "Y")
            //{

            //}

            Console.WriteLine();
            // </SnippetOutputPrediction>
        }

        public static void UseModelWithBatchItems(MLContext mlContext, ITransformer model)
        {
            // Adds some comments to test the trained model's data points.
            // <SnippetCreateTestIssues>
            IEnumerable<SentimentData> sentiments = new[]
            {
                new SentimentData
                {
                    SentimentText = "This was a horrible meal"
                },
                new SentimentData
                {
                    SentimentText = "I love this spaghetti."
                }
            };
            // </SnippetCreateTestIssues>

            // Load batch comments just created 
            // <SnippetPrediction>
            IDataView batchComments = mlContext.Data.LoadFromEnumerable(sentiments);

            IDataView predictions = model.Transform(batchComments);

            // Use model to predict whether comment data is Positive (1) or Negative (0).
            IEnumerable<SentimentPrediction> predictedResults = mlContext.Data.CreateEnumerable<SentimentPrediction>(predictions, reuseRowObject: false);
            // </SnippetPrediction>

            // <SnippetAddInfoMessage>
            Console.WriteLine();

            Console.WriteLine("=============== Prediction Test of loaded model with multiple samples ===============");
            // </SnippetAddInfoMessage>

            Console.WriteLine();

            // <SnippetDisplayResults>
            foreach (SentimentPrediction prediction in predictedResults)
            {
                Console.WriteLine($"Sentiment: {prediction.SentimentText} | Prediction: {prediction.Prediction} | Probability: {prediction.Probability} ");

            }
            Console.WriteLine("=============== End of predictions ===============");
            // </SnippetDisplayResults>       
        }
        public static MLResponse TestSinglePrediction(MLContext mlContext, string sentimentText)
        {

            var currentDir = Directory.GetCurrentDirectory();
            var parentDir = Directory.GetParent(currentDir).FullName;
            var filePath = Path.Combine(parentDir, @"MLAPP\MLModels\SentimentModel.zip");

            ITransformer trainedModel = mlContext.Model.Load(filePath, out var modelInputSchema);
            // <SnippetCreatePredictionEngine1>
            PredictionEngine<SentimentData, SentimentPrediction> predictionFunction = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(trainedModel);
            // </SnippetCreatePredictionEngine1>

            // <SnippetCreateTestIssue1>


            SentimentData sampleStatement = new SentimentData
            {
                SentimentText = sentimentText
            };
            // </SnippetCreateTestIssue1>

            // <SnippetPredict>
            var resultPrediction = predictionFunction.Predict(sampleStatement);
            // </SnippetPredict>
            // <SnippetOutputPrediction>
            Console.WriteLine();
            Console.WriteLine("=============== Prediction Test of model with a single sample and test dataset ===============");

            Console.WriteLine();
            //Console.WriteLine($"Sentiment: {resultPrediction.SentimentText} | Prediction: {(resultPrediction.Prediction)} | Probability: {resultPrediction.Probability} ");
            //string result = ($"For Sentiment: {resultPrediction.SentimentText} | My Prediction is:  {(Convert.ToBoolean(resultPrediction.Prediction) ? "Positive" : "Negative")} | with Probability of: {resultPrediction.Probability} ");
            MLResponse mLResponse = new MLResponse();
            mLResponse.Prediction = Convert.ToBoolean(resultPrediction.Prediction) ? "Positive" : "Negative";
            mLResponse.Probability = resultPrediction.Probability;
            mLResponse.SentimentText = resultPrediction.SentimentText;
            Console.WriteLine();
            Console.WriteLine("=============== End of Predictions ===============");
            // </SnippetOutputPrediction>
            return mLResponse;
        }
        public static void Progress(int p)
        {
            ConsoleHelper.ProgressValue = p;
        }
        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }
    }
}

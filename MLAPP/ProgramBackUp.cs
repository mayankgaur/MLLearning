using Microsoft.ML.Data;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.IO;

namespace MLAPP
{
    class ProgramBackUp
    {
        static string mongoURI = "mongodb://localhost:27017";
        static readonly MongoClient client = new MongoClient(mongoURI);
        delegate void WriteToScreen(int n);
        // <SnippetDeclareGlobalVariables>
        static readonly string _dataPath = Path.Combine(Environment.CurrentDirectory, "Data", "yelp_labelled.txt");
        // </SnippetDeclareGlobalVariables>
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


        //[BsonIgnoreExtraElements]
        //public class SentimentData
        //{
        //    //[BsonId]
        //    //[BsonRepresentation(BsonType.ObjectId)]
        //    //[BsonElement("_id")]
        //    //[LoadColumn(0)]
        //    //public string Id;
        //    //[BsonElement("sentiment")]
        //    //[LoadColumn(1)]
        //    //public float Sentiment;
        //    [LoadColumn(2)]
        //    [BsonElement("text")]
        //    public string SentimentText;
        //    //[LoadColumn(3)]
        //    //public int stars;

        //    [LoadColumn(4),ColumnName("Label")]
        //    public bool Sentiment;
        //    //[LoadColumn(5)]
        //    //[BsonElement("user_id")]
        //    //public string UserId;
        //    //[LoadColumn(6)]
        //    //[BsonElement("business_id")]
        //    //public string BusinessId;

        //}
        //public class SentimentPrediction : SentimentData
        //{
        //    [ColumnName("PredictedLabel")]
        //    public bool Label { get; set; }

        //    public float Probability { get; set; }

        //    public float Score { get; set; }
        //}
        //static void Main(string[] args)
        //{
        //    Color color = Color.FromArgb(130, 150, 115);
        //    //STEP 1: Create MLContext to be shared across the model creation workflow objects
        //    MLContext mlContext = new MLContext();
        //    var db = client.GetDatabase("yelp");
        //    var collection = db.GetCollection<SentimentData>("review_train");
        //    var documents = collection.Find<SentimentData>(new BsonDocument()).ToEnumerable();
        //    //STEP 2: Read data from MongoDB using LoadFromEnumerable and return dataview.
        //    IDataView trainingData = mlContext.Data.LoadFromEnumerable<SentimentData>(documents);
        //    Console.WriteLine("=============== Reading Input Files ===============", color);

        //    // ML.NET doesn't cache data set by default. Therefore, if one reads a data set from a file and accesses it many times, it can be slow due to
        //    // expensive featurization and disk operations. When the considered data can fit into memory, a solution is to cache the data in memory. Caching is especially
        //    // helpful when working with iterative algorithms which needs many data passes. Since SDCA is the case, we cache. Inserting a
        //    // cache step in a pipeline is also possible, please see the construction of pipeline below.
        //    trainingData = mlContext.Data.Cache(trainingData);
        //    Console.WriteLine("=============== Transform Data And Preview ===============", color);
        //    Console.WriteLine();

        //    //STEP 3: Transform your data by encoding the two features userId and movieID.
        //    //        These encoded features will be provided as input to FieldAwareFactorizationMachine learner
        //    //var dataProcessPipeline = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "sentimentIdFeaturized", inputColumnName: nameof(SentimentData.Id))
        //    //                              .Append(mlContext.Transforms.Text.FeaturizeText(outputColumnName: "sentimentFeaturized", inputColumnName: nameof(SentimentData.Sentiment))
        //    //                              .Append(mlContext.Transforms.Concatenate("Features", "sentimentFeaturized", "sentimentIdFeaturized")));


        //    // The Concatenate requires all columns to be of same type.
        //    // If columns are not same then need to convert them to their Single
        //    //var dataProcessPipeline = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "userIdFeaturized", inputColumnName: nameof(SentimentData.UserId))
        //    //                          .Append(mlContext.Transforms.Conversion.ConvertType(outputColumnName: "sentimentStartFeaturized", inputColumnName: nameof(SentimentData.stars)))
        //    //                          .Append(mlContext.Transforms.Concatenate("Features", "userIdFeaturized", "sentimentStartFeaturized"));
        //    //ConsoleHelper.PeekDataViewInConsole(mlContext, trainingData, dataProcessPipeline, 10);
        //    //var dataProcessPipeline = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "userIdFeaturized", inputColumnName: nameof(SentimentData.UserId))
        //    //              .Append(mlContext.Transforms.Text.FeaturizeText(outputColumnName: "businessIdFeaturized", inputColumnName: nameof(SentimentData.BusinessId)))
        //    //              .Append(mlContext.Transforms.Concatenate("Features", "userIdFeaturized", "businessIdFeaturized"));
        //    //ConsoleHelper.PeekDataViewInConsole(mlContext, trainingData, dataProcessPipeline, 10);
        //    //var dataProcessPipeline = mlContext.Transforms.Conversion.ConvertType(outputColumnName: "sentimentStartFeaturized", inputColumnName: nameof(SentimentData.stars), outputKind: DataKind.Single)
        //    //                        .Append(mlContext.Transforms.Text.FeaturizeText(outputColumnName: "sentimentTextFeaturized", inputColumnName: nameof(SentimentData.text)))
        //    //                        .Append(mlContext.Transforms.Concatenate("Features", "sentimentStartFeaturized", "sentimentTextFeaturized"));
        //    //ConsoleHelper.PeekDataViewInConsole(mlContext, trainingData, dataProcessPipeline, 10);
        //    var dataProcessPipeline = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.SentimentText))
        //     .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));
        //    // .Append(mlContext.Transforms.Concatenate("Features",  "sentimentFeaturized"));
        //    ConsoleHelper.PeekDataViewInConsole(mlContext, trainingData, dataProcessPipeline, 10);
        //    // STEP 4: Train the model fitting to the DataSet
        //    Console.WriteLine("=============== Training the model ===============", color);
        //    Console.WriteLine();
        //    var trainingPipeLine = dataProcessPipeline.Append(mlContext.BinaryClassification.Trainers.FieldAwareFactorizationMachine(new string[] { "Features" }));
        //    var model = trainingPipeLine.Fit(trainingData);

        //    //STEP 5: Evaluate the model performance
        //    Console.WriteLine("=============== Evaluating the model ===============", color);
        //    Console.WriteLine();
        //    var testDataView = mlContext.Data.LoadFromEnumerable<SentimentData>(documents);

        //    var prediction = model.Transform(testDataView);
        //    var preViewTransformedData = prediction.Preview(maxRows: 20);
        //    Console.WriteLine("=============== Preview Transform Data Starts ===============", color);
        //    foreach (var row in preViewTransformedData.RowView)
        //    {
        //        var ColumnCollection = row.Values;
        //        string lineToPrint = "Row--> ";
        //        foreach (KeyValuePair<string, object> column in ColumnCollection)
        //        {
        //            lineToPrint += $"| {column.Key}:{column.Value}";
        //        }
        //        Console.WriteLine(lineToPrint + "\n");
        //    }
        //    Console.WriteLine("=============== Preview Transform Data Ends ===============", color);


        //    var metrics = mlContext.BinaryClassification.Evaluate(data: prediction, labelColumnName: "Label", scoreColumnName: "Score", predictedLabelColumnName: "PredictedLabel");
        //    // var metrics = mlContext.BinaryClassification.Evaluate(data: prediction);
        //    Console.WriteLine("Evaluation Metrics: acc:" + Math.Round(metrics.Accuracy, 2) + " AreaUnderRocCurve(AUC):" + Math.Round(metrics.AreaUnderRocCurve, 2), color);
        //    //Console.WriteLine("Evaluation Metrics: acc:" + Math.Round(metrics.Accuracy, 2));

        //    //STEP 6:  Try/test a single prediction by predicting a single movie rating for a specific user
        //    Console.WriteLine("=============== Test a single prediction ===============", color);
        //    Console.WriteLine();
        //    var predictionEngine = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
        //    SentimentData testData = new SentimentData() { SentimentText="Wow what a Pizaa !!!" };
        //    var movieRatingPrediction = predictionEngine.Predict(testData);
        //    Console.WriteLine($"SentimentText:{testData.SentimentText} with Sentiment: {testData.Sentiment} Score:{Sigmoid(movieRatingPrediction.Score)}, Prediction Label {movieRatingPrediction.Prediction} and Probability {movieRatingPrediction.Probability}", Color.YellowGreen);
        //    Console.WriteLine();


        //    Console.ReadLine();
        //}
        //public static float Sigmoid(float x)
        //{
        //    return (float)(100 / (1 + Math.Exp(-x)));
        //}

        //static void Main(string[] args)
        //{
        //    // Create ML.NET context/local environment - allows you to add steps in order to keep everything together 
        //    // as you discover the ML.NET trainers and transforms 
        //    // <SnippetCreateMLContext>
        //    MLContext mlContext = new MLContext();
        //    // </SnippetCreateMLContext>

        //    // <SnippetCallLoadData>
        //    TrainTestData splitDataView = LoadData(mlContext);
        //    // </SnippetCallLoadData>


        //    // <SnippetCallBuildAndTrainModel>
        //    ITransformer model = BuildAndTrainModel(mlContext, splitDataView.TrainSet);
        //    // </SnippetCallBuildAndTrainModel>

        //    // <SnippetCallEvaluate>
        //    Evaluate(mlContext, model, splitDataView.TestSet);
        //    // </SnippetCallEvaluate>

        //    // <SnippetCallUseModelWithSingleItem>
        //    UseModelWithSingleItem(mlContext, model);
        //    // </SnippetCallUseModelWithSingleItem>

        //    // <SnippetCallUseModelWithBatchItems>
        //    UseModelWithBatchItems(mlContext, model);
        //    // </SnippetCallUseModelWithBatchItems>

        //    Console.WriteLine();
        //    Console.WriteLine("=============== End of process ===============");
        //}

        //public static TrainTestData LoadData(MLContext mlContext)
        //{
        //    // Note that this case, loading your training data from a file, 
        //    // is the easiest way to get started, but ML.NET also allows you 
        //    // to load data from databases or in-memory collections.
        //    // <SnippetLoadData>

        //    var db = client.GetDatabase("yelp");
        //    var collection = db.GetCollection<SentimentData>("review_train");
        //    var documents = collection.Find<SentimentData>(new BsonDocument()).ToEnumerable();
        //    //STEP 2: Read data from MongoDB using LoadFromEnumerable and return dataview.
        //    IDataView dataView = mlContext.Data.LoadFromEnumerable<SentimentData>(documents);

        //    // </SnippetLoadData>

        //    // You need both a training dataset to train the model and a test dataset to evaluate the model.
        //    // Split the loaded dataset into train and test datasets
        //    // Specify test dataset percentage with the `testFraction`parameter
        //    // <SnippetSplitData>
        //    TrainTestData splitDataView = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
        //    // </SnippetSplitData>

        //    // <SnippetReturnSplitData>        
        //    return splitDataView;
        //    // </SnippetReturnSplitData>           
        //}

        //public static ITransformer BuildAndTrainModel(MLContext mlContext, IDataView splitTrainSet)
        //{
        //    // Create a flexible pipeline (composed by a chain of estimators) for creating/training the model.
        //    // This is used to format and clean the data.  
        //    // Convert the text column to numeric vectors (Features column) 
        //    // <SnippetFeaturizeText>
        //    var estimator = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.SentimentText))
        //    //</SnippetFeaturizeText>
        //    // append the machine learning task to the estimator
        //    // <SnippetAddTrainer> 
        //    .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));
        //    // </SnippetAddTrainer>

        //    // Create and train the model based on the dataset that has been loaded, transformed.
        //    // <SnippetTrainModel>
        //    Console.WriteLine("=============== Create and Train the Model ===============");
        //    var model = estimator.Fit(splitTrainSet);
        //    Console.WriteLine("=============== End of training ===============");
        //    Console.WriteLine();
        //    // </SnippetTrainModel>

        //    // Returns the model we trained to use for evaluation.
        //    // <SnippetReturnModel>
        //    return model;
        //    // </SnippetReturnModel>
        //}

        //public static void Evaluate(MLContext mlContext, ITransformer model, IDataView splitTestSet)
        //{
        //    Color color = Color.FromArgb(130, 150, 115);
        //    // Evaluate the model and show accuracy stats

        //    //Take the data in, make transformations, output the data. 
        //    // <SnippetTransformData>
        //    Console.WriteLine("=============== Evaluating Model accuracy with Test data===============");
        //    IDataView predictions = model.Transform(splitTestSet);
        //    var preViewTransformedData = predictions.Preview(maxRows: 20);
        //    Console.WriteLine("=============== Preview Transform Data Starts ===============", color);
        //    foreach (var row in preViewTransformedData.RowView)
        //    {
        //        var ColumnCollection = row.Values;
        //        string lineToPrint = "Row--> ";
        //        foreach (KeyValuePair<string, object> column in ColumnCollection)
        //        {
        //            lineToPrint += $"| {column.Key}:{column.Value}";
        //        }
        //        Console.WriteLine(lineToPrint + "\n", color);
        //    }
        //    Console.WriteLine("=============== Preview Transform Data Ends ===============", color);
        //    // </SnippetTransformData>

        //    // BinaryClassificationContext.Evaluate returns a BinaryClassificationEvaluator.CalibratedResult
        //    // that contains the computed overall metrics.
        //    // <SnippetEvaluate>
        //    CalibratedBinaryClassificationMetrics metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");
        //    // </SnippetEvaluate>

        //    // The Accuracy metric gets the accuracy of a model, which is the proportion 
        //    // of correct predictions in the test set.

        //    // The AreaUnderROCCurve metric is equal to the probability that the algorithm ranks
        //    // a randomly chosen positive instance higher than a randomly chosen negative one
        //    // (assuming 'positive' ranks higher than 'negative').

        //    // The F1Score metric gets the model's F1 score.
        //    // The F1 score is the harmonic mean of precision and recall:
        //    //  2 * precision * recall / (precision + recall).

        //    // <SnippetDisplayMetrics>
        //    Console.WriteLine();
        //    Console.WriteLine("Model quality metrics evaluation");
        //    Console.WriteLine("--------------------------------");
        //    Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
        //    Console.WriteLine($"Auc: {metrics.AreaUnderRocCurve:P2}");
        //    Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
        //    Console.WriteLine("=============== End of model evaluation ===============");
        //    //</SnippetDisplayMetrics>

        //}

        //private static void UseModelWithSingleItem(MLContext mlContext, ITransformer model)
        //{
        //    // <SnippetCreatePredictionEngine1>
        //    PredictionEngine<SentimentData, SentimentPrediction> predictionFunction = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
        //    // </SnippetCreatePredictionEngine1>

        //    // <SnippetCreateTestIssue1>
        //    SentimentData sampleStatement = new SentimentData
        //    {
        //        SentimentText = "This was a very bad steak"
        //    };
        //    // </SnippetCreateTestIssue1>

        //    // <SnippetPredict>
        //    var resultPrediction = predictionFunction.Predict(sampleStatement);
        //    // </SnippetPredict>
        //    // <SnippetOutputPrediction>
        //    Console.WriteLine();
        //    Console.WriteLine("=============== Prediction Test of model with a single sample and test dataset ===============");

        //    Console.WriteLine();
        //    Console.WriteLine($"Sentiment: {resultPrediction.SentimentText} | Prediction: {(Convert.ToBoolean(resultPrediction.Prediction) ? "Positive" : "Negative")} | Probability: {resultPrediction.Probability} ");

        //    Console.WriteLine("=============== End of Predictions ===============");
        //    Console.WriteLine();
        //    // </SnippetOutputPrediction>
        //}

        //public static void UseModelWithBatchItems(MLContext mlContext, ITransformer model)
        //{
        //    // Adds some comments to test the trained model's data points.
        //    // <SnippetCreateTestIssues>
        //    IEnumerable<SentimentData> sentiments = new[]
        //    {
        //        new SentimentData
        //        {
        //            SentimentText = "This was a horrible meal"
        //        },
        //        new SentimentData
        //        {
        //            SentimentText = "I love this spaghetti."
        //        }
        //    };
        //    // </SnippetCreateTestIssues>

        //    // Load batch comments just created 
        //    // <SnippetPrediction>
        //    IDataView batchComments = mlContext.Data.LoadFromEnumerable(sentiments);

        //    IDataView predictions = model.Transform(batchComments);

        //    // Use model to predict whether comment data is Positive (1) or Negative (0).
        //    IEnumerable<SentimentPrediction> predictedResults = mlContext.Data.CreateEnumerable<SentimentPrediction>(predictions, reuseRowObject: false);
        //    // </SnippetPrediction>

        //    // <SnippetAddInfoMessage>
        //    Console.WriteLine();

        //    Console.WriteLine("=============== Prediction Test of loaded model with multiple samples ===============");
        //    // </SnippetAddInfoMessage>

        //    Console.WriteLine();

        //    // <SnippetDisplayResults>
        //    foreach (SentimentPrediction prediction in predictedResults)
        //    {
        //        Console.WriteLine($"Sentiment: {prediction.SentimentText} | Prediction: {(Convert.ToBoolean(prediction.Prediction) ? "Positive" : "Negative")} | Probability: {prediction.Probability} ");

        //    }
        //    Console.WriteLine("=============== End of predictions ===============");
        //    // </SnippetDisplayResults>       
        //}

        //static void Main(string[] args)
        //{
        //    // Create ML.NET context/local environment - allows you to add steps in order to keep everything together 
        //    // as you discover the ML.NET trainers and transforms 
        //    // <SnippetCreateMLContext>

        //    MLContext mlContext = new MLContext();
        //    // </SnippetCreateMLContext>
        //    Color color = Color.FromArgb(130, 150, 115);
        //    Console.WriteLine("=============== Reading DB Starts ===============", color);
        //    // <SnippetCallLoadData>
        //    TrainTestData splitDataView = LoadData(mlContext);
        //    // </SnippetCallLoadData>


        //    // <SnippetCallBuildAndTrainModel>
        //    ITransformer model = BuildAndTrainModel(mlContext, splitDataView.TrainSet);
        //    // </SnippetCallBuildAndTrainModel>

        //    // <SnippetCallEvaluate>
        //    Evaluate(mlContext, model, splitDataView.TestSet);
        //    // </SnippetCallEvaluate>

        //    // <SnippetCallUseModelWithSingleItem>
        //    UseModelWithSingleItem(mlContext, model);
        //    // </SnippetCallUseModelWithSingleItem>

        //    // <SnippetCallUseModelWithBatchItems>
        //    UseModelWithBatchItems(mlContext, model);
        //    // </SnippetCallUseModelWithBatchItems>

        //    Console.WriteLine();
        //    Console.WriteLine("=============== End of process ===============");
        //    Console.ReadLine();
        //}

        //public static TrainTestData LoadData(MLContext mlContext)
        //{
        //    // Note that this case, loading your training data from a file, 
        //    // is the easiest way to get started, but ML.NET also allows you 
        //    // to load data from databases or in-memory collections.
        //    // <SnippetLoadData>
        //    Color color = Color.FromArgb(130, 150, 115);
        //    Console.WriteLine("=============== DB Opening ===============", color);
        //    var db = client.GetDatabase("yelp");
        //    var collection = db.GetCollection<SentimentData>("review_train");
        //    var documents = collection.Find<SentimentData>(new BsonDocument()).ToEnumerable();
        //    Console.WriteLine("=============== Data Collected ===============", color);
        //    //STEP 2: Read data from MongoDB using LoadFromEnumerable and return dataview.
        //    // IDataView trainingData = mlContext.Data.LoadFromTextFile<SentimentData>(_dataPath, hasHeader: false);
        //    IDataView trainingData = mlContext.Data.LoadFromEnumerable<SentimentData>(documents);
        //    Console.WriteLine("=============== Data Loaded to Training Data ===============", color);
        //    // </SnippetLoadData>

        //    // You need both a training dataset to train the model and a test dataset to evaluate the model.
        //    // Split the loaded dataset into train and test datasets
        //    // Specify test dataset percentage with the `testFraction`parameter
        //    // <SnippetSplitData>
        //    TrainTestData splitDataView = mlContext.Data.TrainTestSplit(trainingData, testFraction: 0.2);
        //    // </SnippetSplitData>

        //    // <SnippetReturnSplitData>        
        //    return splitDataView;
        //    // </SnippetReturnSplitData>           
        //}

        //public static ITransformer BuildAndTrainModel(MLContext mlContext, IDataView splitTrainSet)
        //{
        //    // Create a flexible pipeline (composed by a chain of estimators) for creating/training the model.
        //    // This is used to format and clean the data.  
        //    // Convert the text column to numeric vectors (Features column) 
        //    // <SnippetFeaturizeText>
        //    var estimator = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.SentimentText))
        //    //</SnippetFeaturizeText>
        //    // append the machine learning task to the estimator
        //    // <SnippetAddTrainer> 
        //    .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));
        //    // </SnippetAddTrainer>

        //    // Create and train the model based on the dataset that has been loaded, transformed.
        //    // <SnippetTrainModel>
        //    Console.WriteLine("=============== Create and Train the Model ===============");
        //    var model = estimator.Fit(splitTrainSet);
        //    Console.WriteLine("=============== End of training ===============");
        //    Console.WriteLine();
        //    // </SnippetTrainModel>

        //    // Returns the model we trained to use for evaluation.
        //    // <SnippetReturnModel>
        //    return model;
        //    // </SnippetReturnModel>
        //}
        //public static void Progress(int p)
        //{
        //    ConsoleHelper.ProgressValue = p;


        //}
        //public static void Evaluate(MLContext mlContext, ITransformer model, IDataView splitTestSet)
        //{
        //    // Evaluate the model and show accuracy stats
        //    Color color = Color.FromArgb(130, 150, 115);
        //    //Take the data in, make transformations, output the data. 
        //    // <SnippetTransformData>
        //    Console.WriteLine("=============== Evaluating Model accuracy with Test data===============");
        //    IDataView predictions = model.Transform(splitTestSet);
        //    var preViewTransformedData = predictions.Preview();
        //    Console.WriteLine("=============== Preview Transform Data Starts ===============", color);

        //    ConsoleHelper.ProgressTotal = preViewTransformedData.RowView.Length;
        //    int i = 1;
        //    foreach (var row in preViewTransformedData.RowView)
        //    {

        //        var ColumnCollection = row.Values;
        //        string lineToPrint = "Row--> ";

        //        foreach (KeyValuePair<string, object> column in ColumnCollection)
        //        {                                     
        //               lineToPrint += $"| {column.Key}:{column.Value}";                    
        //        }
        //        //WriteToScreen nc1 = new WriteToScreen(Progress);
        //        //nc1(i);
        //        //i++;
        //        Console.WriteLine(lineToPrint + "\n", color);
        //    }

        //    // </SnippetTransformData>

        //    // BinaryClassificationContext.Evaluate returns a BinaryClassificationEvaluator.CalibratedResult
        //    // that contains the computed overall metrics.
        //    // <SnippetEvaluate>
        //    CalibratedBinaryClassificationMetrics metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");
        //    // </SnippetEvaluate>

        //    // The Accuracy metric gets the accuracy of a model, which is the proportion 
        //    // of correct predictions in the test set.

        //    // The AreaUnderROCCurve metric is equal to the probability that the algorithm ranks
        //    // a randomly chosen positive instance higher than a randomly chosen negative one
        //    // (assuming 'positive' ranks higher than 'negative').

        //    // The F1Score metric gets the model's F1 score.
        //    // The F1 score is the harmonic mean of precision and recall:
        //    //  2 * precision * recall / (precision + recall).

        //    // <SnippetDisplayMetrics>
        //    Console.WriteLine();
        //    Console.WriteLine("Model quality metrics evaluation");
        //    Console.WriteLine("--------------------------------");
        //    Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
        //    Console.WriteLine($"Auc: {metrics.AreaUnderRocCurve:P2}");
        //    Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
        //    Console.WriteLine("=============== End of model evaluation ===============");
        //    //</SnippetDisplayMetrics>

        //}

        //private static void UseModelWithSingleItem(MLContext mlContext, ITransformer model)
        //{
        //    // <SnippetCreatePredictionEngine1>
        //    PredictionEngine<SentimentData, SentimentPrediction> predictionFunction = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
        //    // </SnippetCreatePredictionEngine1>

        //    // <SnippetCreateTestIssue1>
        //    SentimentData sampleStatement = new SentimentData
        //    {
        //        SentimentText = "This was a very bad steak"
        //    };
        //    // </SnippetCreateTestIssue1>

        //    // <SnippetPredict>
        //    var resultPrediction = predictionFunction.Predict(sampleStatement);
        //    // </SnippetPredict>
        //    // <SnippetOutputPrediction>
        //    Console.WriteLine();
        //    Console.WriteLine("=============== Prediction Test of model with a single sample and test dataset ===============");

        //    Console.WriteLine();
        //    Console.WriteLine($"Sentiment: {resultPrediction.SentimentText} | Prediction: {(resultPrediction.Prediction)} | Probability: {resultPrediction.Probability} ");

        //    Console.WriteLine("=============== End of Predictions ===============");
        //    Console.WriteLine();
        //    // </SnippetOutputPrediction>
        //}

        //public static void UseModelWithBatchItems(MLContext mlContext, ITransformer model)
        //{
        //    // Adds some comments to test the trained model's data points.
        //    // <SnippetCreateTestIssues>
        //    IEnumerable<SentimentData> sentiments = new[]
        //    {
        //        new SentimentData
        //        {
        //            SentimentText = "This was a horrible meal"
        //        },
        //        new SentimentData
        //        {
        //            SentimentText = "I love this spaghetti."
        //        }
        //    };
        //    // </SnippetCreateTestIssues>

        //    // Load batch comments just created 
        //    // <SnippetPrediction>
        //    IDataView batchComments = mlContext.Data.LoadFromEnumerable(sentiments);

        //    IDataView predictions = model.Transform(batchComments);

        //    // Use model to predict whether comment data is Positive (1) or Negative (0).
        //    IEnumerable<SentimentPrediction> predictedResults = mlContext.Data.CreateEnumerable<SentimentPrediction>(predictions, reuseRowObject: false);
        //    // </SnippetPrediction>

        //    // <SnippetAddInfoMessage>
        //    Console.WriteLine();

        //    Console.WriteLine("=============== Prediction Test of loaded model with multiple samples ===============");
        //    // </SnippetAddInfoMessage>

        //    Console.WriteLine();

        //    // <SnippetDisplayResults>
        //    foreach (SentimentPrediction prediction in predictedResults)
        //    {
        //        Console.WriteLine($"Sentiment: {prediction.SentimentText} | Prediction: {prediction.Prediction} | Probability: {prediction.Probability} ");

        //    }
        //    Console.WriteLine("=============== End of predictions ===============");
        //    // </SnippetDisplayResults>       
        //}





    }

}

//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Microsoft.AspNetCore.Http;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Brokers.Azures
{
    public class AzureBroker : IAzureBroker
    {
        public async ValueTask<string> TakeFeedbackAsync(string formFile)
        {
            SpeechConfig config = SpeechConfig.FromSubscription("b0865984f22d42ebb91601daa3eb27a7", "eastus");
            using var audioConfig = AudioConfig.FromWavFileInput(formFile);
            string language = "en-US";
            string topic = "your own topic";

            var speechRecognizer = new SpeechRecognizer(config, language.Replace("_", "-"), audioConfig);

            var connection = Connection.FromRecognizer(speechRecognizer);

            var phraseDetectionConfig = new
            {
                enrichment = new
                {
                    pronunciationAssessment = new
                    {
                        referenceText = "",
                        gradingSystem = "HundredMark",
                        granularity = "Word",
                        dimension = "Comprehensive",
                        enableMiscue = "False",
                        enableProsodyAssessment = "True"
                    },
                    contentAssessment = new
                    {
                        topic = topic
                    }
                }
            };
            connection.SetMessageProperty("speech.context", "phraseDetection", JsonConvert.SerializeObject(phraseDetectionConfig));

            var phraseOutputConfig = new
            {
                format = "Detailed",
                detailed = new
                {
                    options = new[]
                    {
                    "WordTimings",
                    "PronunciationAssessment",
                    "ContentAssessment",
                    "SNR",
                }
                }
            };

            connection.SetMessageProperty("speech.context", "phraseOutput", JsonConvert.SerializeObject(phraseOutputConfig));

            var done = false;
            var fullRecognizedText = "";

            speechRecognizer.SessionStopped += (s, e) =>
            {
                Console.WriteLine("Closing on {0}", e);
                done = true;
            };

            speechRecognizer.Canceled += (s, e) =>
            {
                Console.WriteLine("Closing on {0}", e);
                done = true;
            };

            string result = null;

            connection.MessageReceived += (s, e) =>
            {
                if (e.Message.IsTextMessage())
                {
                    var messageText = e.Message.GetTextMessage();
                    var json = Newtonsoft.Json.Linq.JObject.Parse(messageText);
                    if (json.ContainsKey("NBest"))
                    {
                        var nBest = json["NBest"][0];
                        if (nBest["Display"].ToString().Trim().Length > 1)
                        {
                            var recognizedText = json["DisplayText"];

                            fullRecognizedText += $" {recognizedText}";
                            var accuracyScore = nBest["PronunciationAssessment"]["AccuracyScore"].ToString();
                            var fluencyScore = nBest["PronunciationAssessment"]["FluencyScore"].ToString();
                            var prosodyScore = nBest["PronunciationAssessment"]["ProsodyScore"].ToString();
                            var completenessScore = nBest["PronunciationAssessment"]["CompletenessScore"].ToString();
                            var pronScore = nBest["PronunciationAssessment"]["PronScore"].ToString();

                            result = $"Accuracy Score {accuracyScore}\n" +
                             $"Fluency Score: {fluencyScore}\n" +
                             $"Prosody Score {prosodyScore}\n" +
                             $"Completeness Score {completenessScore}\n" +
                             $"PronScore {pronScore}";

                        }
                    }
                }
            };
            await speechRecognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

            while (!done)
            {
                await Task.Delay(1000);
            }

            await speechRecognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);

            return result;
        }
    }
}

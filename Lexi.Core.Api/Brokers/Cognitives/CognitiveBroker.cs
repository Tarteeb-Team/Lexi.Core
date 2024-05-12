//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Services.Foundations.TelegramHandles;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.PronunciationAssessment;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Brokers.Cognitives
{
    public class CognitiveBroker : ICognitiveBroker
    {
        string speechKey = "aec1b94cf0254f11b478d28a50743eeb";
        string speechRegion = "eastus";
        private readonly ITelegramHandleService telegramBroker;

        public CognitiveBroker(ITelegramHandleService telegramBroker)
        {
            this.telegramBroker = telegramBroker;
        }

        public async Task<string> GetJsonString()
        {
            string _filePath = this.telegramBroker.ReturnFilePath();

            var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
            speechConfig.SpeechRecognitionLanguage = "en-US";

            var pronunciationAssessmentConfig = new PronunciationAssessmentConfig(
            referenceText: "",
            gradingSystem: GradingSystem.HundredMark,
            granularity: Granularity.Phoneme,
            enableMiscue: false);
            pronunciationAssessmentConfig.EnableProsodyAssessment();
            pronunciationAssessmentConfig.EnableContentAssessmentWithTopic("greeting");

            using var audioConfig = AudioConfig.FromWavFileInput(_filePath);

            // Creating a SpeechRecognizer using SpeechConfig and AudioConfig
            using (var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig))
            {
                pronunciationAssessmentConfig.ApplyTo(speechRecognizer);
                var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();

                // The pronunciation assessment result as a Speech SDK object
                var pronunciationAssessmentResult =
                         PronunciationAssessmentResult.FromResult(speechRecognitionResult);

                // The pronunciation assessment result as a JSON string
                var pronunciationAssessmentResultJson = speechRecognitionResult
                    .Properties.GetProperty(PropertyId.SpeechServiceResponse_JsonResult);

                return pronunciationAssessmentResultJson;
            }
        }

    }
}

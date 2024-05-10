﻿using Lexi.Core.Api.Services.Foundations.Telegrams;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Brokers.Speeches
{
    public partial class SpeechBroker : ISpeechBroker
    {
        private readonly SpeechConfig speechConfig;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly string wwwRootPath;

        public SpeechBroker(
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.wwwRootPath = this.webHostEnvironment.WebRootPath;
            this.speechConfig = SpeechConfig.FromSubscription(
                subscriptionKey: "aec1b94cf0254f11b478d28a50743eeb",
                region: "eastus");

            this.speechConfig.SpeechSynthesisVoiceName = "en-US-EmmaNeural";
        }


        public async ValueTask<SpeechSynthesisResult> GetSpeechResultAsync(string text)
        {
            using (var speechSynthesizer = new SpeechSynthesizer(speechConfig, null))
            {
                SpeechSynthesisResult speechResult =
                    await speechSynthesizer.SpeakTextAsync(text: text);

                return speechResult;
            }
        }

        public async Task<string> RecognizeSpeechAsync(string audioFilePath)
        {
            using (var audioConfig = AudioConfig.FromWavFileInput(audioFilePath))
            {
                using (var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig))
                {
                    var result = await speechRecognizer.RecognizeOnceAsync();
                    return result.Text;
                }
            }
        }
    }
}

using Lexi.Core.Api.Brokers.Speeches;
using Lexi.Core.Api.Services.Foundations.Telegrams;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CognitiveServices.Speech;
using System;
using System.IO;
using System.Threading.Tasks;

namespace aisha_ai.Services.Foundations.HandleSpeeches
{
    public class HandleSpeechService : IHandleSpeechService
    {
        private readonly ISpeechBroker speechBroker;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ITelegramService telegramService;
        private readonly string wwwRootPath;

        public HandleSpeechService(
            ISpeechBroker speechBroker,
            IWebHostEnvironment webHostEnvironment,
            ITelegramService telegramService)
        {
            this.speechBroker = speechBroker;
            this.webHostEnvironment = webHostEnvironment;
            this.wwwRootPath = this.webHostEnvironment.WebRootPath;
            this.telegramService = telegramService;
        }

        public async ValueTask<string> CreateAndSaveSpeechAudioAsync(string text, string fileName)
        {
                text = text.Replace("\n", "").Replace("\t", "").Replace("*", "").Replace("\\\"", "").Replace("/", "");
                string audioFolderPath = Path.Combine(this.wwwRootPath, "AiVoices" ,$"{fileName}.wav");

                SpeechSynthesisResult speechSynthesisResult =
                    await this.speechBroker.GetSpeechResultAsync(text, "");

                await SaveSpeechSynthesisResultToLocalDirectoryAsync(
                           speechSynthesisResult: speechSynthesisResult,
                           filePath: audioFolderPath);


                return audioFolderPath;
        }
        
        public async ValueTask<string> CreateAndSaveSpeechAudioPartOneAsync(string text, string fileName)
        {
                text = text.Replace("\n", "").Replace("\t", "").Replace("*", "").Replace("\\\"", "").Replace("/", "");
                string audioFolderPath = Path.Combine(this.wwwRootPath, "PartOneFeedback", $"{fileName}.wav");

                SpeechSynthesisResult speechSynthesisResult =
                    await this.speechBroker.GetSpeechResultAsync(text, "");

                await SaveSpeechSynthesisResultToLocalDirectoryAsync(
                           speechSynthesisResult: speechSynthesisResult,
                           filePath: audioFolderPath);


                return audioFolderPath;
        }


        private async Task SaveSpeechSynthesisResultToLocalDirectoryAsync(
        SpeechSynthesisResult speechSynthesisResult,
        string filePath)
        {
            if (speechSynthesisResult.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                try
                {
                    using (var audioStream = AudioDataStream.FromResult(speechSynthesisResult))
                    {
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }

                        await audioStream.SaveToWaveFileAsync(filePath);
                    }

                    speechSynthesisResult.Dispose();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

    }
}

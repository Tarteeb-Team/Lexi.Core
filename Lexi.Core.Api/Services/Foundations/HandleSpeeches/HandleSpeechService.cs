using Lexi.Core.Api.Brokers.Speeches;
using Lexi.Core.Api.Brokers.UpdateStorages;
using Lexi.Core.Api.Models.Foundations.QuestionTypes;
using Lexi.Core.Api.Services.Foundations.Telegrams;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CognitiveServices.Speech;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace aisha_ai.Services.Foundations.HandleSpeeches
{
    public class HandleSpeechService : IHandleSpeechService
    {
        private readonly ISpeechBroker speechBroker;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ITelegramService telegramService;
        private readonly string wwwRootPath;
        private readonly IUpdateStorageBroker updateStorageBroker;

        public HandleSpeechService(
            ISpeechBroker speechBroker,
            IWebHostEnvironment webHostEnvironment,
            ITelegramService telegramService,
            IUpdateStorageBroker updateStorageBroker)
        {
            this.speechBroker = speechBroker;
            this.webHostEnvironment = webHostEnvironment;
            this.wwwRootPath = this.webHostEnvironment.WebRootPath;
            this.telegramService = telegramService;
            this.updateStorageBroker = updateStorageBroker;
        }

        public async ValueTask<string> CreateAndSaveSpeechAudioAsync(string text, string fileName)
        {
                text = text.Replace("\n", "").Replace("\t", "").Replace("*", "").Replace("\\\"", "").Replace("/", "");
                string audioFolderPath = Path.Combine(this.wwwRootPath, "AiVoices" ,$"{fileName}.wav");

            long telegramId = Convert.ToInt64(fileName);

            QuestionType? voiceType = this.updateStorageBroker.SelectAllQuestionTypes()
                .FirstOrDefault(q => q.TelegramId == telegramId);

            string voice = "en-IN-NeerjaNeura";

            if (voiceType.Type is not null || voiceType is not null)
            {
                voice = voiceType.Type;
            }

            SpeechSynthesisResult speechSynthesisResult =
                    await this.speechBroker.GetSpeechResultAsync(text, voice);

                await SaveSpeechSynthesisResultToLocalDirectoryAsync(
                           speechSynthesisResult: speechSynthesisResult,
                           filePath: audioFolderPath);


                return audioFolderPath;
        }
        
        public async ValueTask<string> CreateAndSaveSpeechAudioPartOneAsync(string text, string fileName)
        {
                text = text.Replace("\n", "").Replace("\t", "").Replace("*", "").Replace("\\\"", "").Replace("/", "");
                string audioFolderPath = Path.Combine(this.wwwRootPath, "PartOneFeedback", $"{fileName}.wav");

            long telegramId = Convert.ToInt64(fileName);

            QuestionType? voiceType = this.updateStorageBroker.SelectAllQuestionTypes()
                .FirstOrDefault(q => q.TelegramId == telegramId);

            string voice = "en-IN-NeerjaNeura";

            if (voiceType.Type is not null || voiceType is not null)
            {
                voice = voiceType.Type;
            }

            SpeechSynthesisResult speechSynthesisResult =
                    await this.speechBroker.GetSpeechResultAsync(text, voice);

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

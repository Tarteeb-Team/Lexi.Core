using Microsoft.CognitiveServices.Speech;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Lexi.Core.Api.Brokers.Speeches
{
    public partial class SpeechBroker
    {
        public async ValueTask<string> CreateAndSaveSpeechAudioPartOneAsync(string text, string fileName)
        {
            text = text.Replace("\n", "").Replace("\t", "").Replace("*", "").Replace("\\\"", "").Replace("/", "");
            string audioFolderPath = Path.Combine(this.wwwRootPath, "PartOneFeedback", $"{fileName}.wav");

            long telegramId = Convert.ToInt64(fileName);

            var voiceType = this.updateStorageBroker.SelectAllQuestionTypes()
                .FirstOrDefault(q => q.TelegramId == telegramId);

            SpeechSynthesisResult speechSynthesisResult = await GetSpeechResultAsync(text, voiceType.Type);

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

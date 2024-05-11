using Microsoft.CognitiveServices.Speech;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Brokers.Speeches
{
    public interface ISpeechBroker
    {
        ValueTask<SpeechSynthesisResult> GetSpeechResultAsync(string text, string voiceType);
        ValueTask<string> CreateAndSaveSpeechAudioPartOneAsync(string text, string fileName);
        Task<string> RecognizeSpeechAsync(string audioFilePath);
    }
}

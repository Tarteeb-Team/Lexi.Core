using System.Threading.Tasks;

namespace aisha_ai.Services.Foundations.HandleSpeeches
{
    public interface IHandleSpeechService
    {
        ValueTask<string> CreateAndSaveSpeechAudioAsync(string text, string fileName);
        ValueTask<string> CreateAndSaveSpeechAudioPartOneAsync(string text, string fileName);

    }
}

using System.Threading.Tasks;

namespace Lexi.Core.Api.Services.Foundations.ImproveSpeech
{
    public interface IOpenAIService
    {
        ValueTask<string> AnalizeRequestAsync(string text, string promt);
    }
}

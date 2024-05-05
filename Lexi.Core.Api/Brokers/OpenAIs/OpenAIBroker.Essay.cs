using Standard.AI.OpenAI.Models.Services.Foundations.ChatCompletions;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Brokers.OpenAIs
{
    public partial class OpenAIBroker
    {
        public async ValueTask<ChatCompletion> AnalyzeEssayAsync(ChatCompletion chatCompletion)
        {
            return await this.openAiClient.ChatCompletions.SendChatCompletionAsync(
                    chatCompletion);
        }
    }
}

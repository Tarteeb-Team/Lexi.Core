using Standard.AI.OpenAI.Models.Services.Foundations.ChatCompletions;
using System;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Brokers.OpenAIs
{
    public partial class OpenAIBroker
    {
        public async ValueTask<ChatCompletion> AnalyzeEssayAsync(ChatCompletion chatCompletion)
        {
            try
            {

                return await this.openAiClient.ChatCompletions.SendChatCompletionAsync(
                        chatCompletion);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);

                throw;
            }
        }
    }
}

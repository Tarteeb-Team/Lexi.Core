using Lexi.Core.Api.Brokers.OpenAIs;
using Standard.AI.OpenAI.Models.Services.Foundations.ChatCompletions;
using System.Linq;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Services.Foundations.ImproveSpeech
{
    public class OpenAIService : IOpenAIService
    {
        private readonly IOpenAIBroker openAiBroker;

        public OpenAIService(IOpenAIBroker openAiBroker) =>
            this.openAiBroker = openAiBroker;

        public async ValueTask<string> AnalizeRequestAsync(string text, string promt)
        {
            ChatCompletion request = CreateRequest(text, promt);
            ChatCompletion result = await openAiBroker.AnalyzeEssayAsync(request);

            return result.Response.Choices.FirstOrDefault().Message.Content;
        }

        private static ChatCompletion CreateRequest(string text, string promt)
        {
            return new ChatCompletion
            {
                Request = new ChatCompletionRequest
                {
                    Model = "gpt-4-1106-preview",
                    MaxTokens = 1500,
                    Messages = new ChatCompletionMessage[]
                   {
                       new ChatCompletionMessage
                        {
                            Content = promt,

                            Role = "system",
                        },
                        new ChatCompletionMessage
                        {
                            Content = text, 
                            Role = "user",
                        }
                   },
                }
            };
        }
    }
}

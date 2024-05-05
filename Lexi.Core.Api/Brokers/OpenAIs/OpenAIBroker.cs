using Microsoft.Extensions.Configuration;
using Standard.AI.OpenAI.Clients.OpenAIs;
using Standard.AI.OpenAI.Models.Configurations;

namespace Lexi.Core.Api.Brokers.OpenAIs
{
    public partial class OpenAIBroker : IOpenAIBroker
    {
        private readonly OpenAIClient openAiClient;
        private readonly IConfiguration configuration;

        public OpenAIBroker(
            IConfiguration configuration)
        {
            this.configuration = configuration;
            this.openAiClient = ConfigureOpenAIClient();
        }

        private OpenAIClient ConfigureOpenAIClient()
        {
            string apiKey = this.configuration.GetValue<string>(key: "OpenAiKey");

            var openAIConfigurations = new OpenAIConfigurations
            {
                ApiKey = apiKey
            };

            return new OpenAIClient(openAIConfigurations);
        }
    }
}

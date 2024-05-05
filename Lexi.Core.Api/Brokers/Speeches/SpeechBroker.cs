using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Brokers.Speeches
{
    public class SpeechBroker : ISpeechBroker
    {
        private readonly SpeechConfig speechConfig;

        public SpeechBroker(
            IConfiguration configuration)
        {
            this.speechConfig = SpeechConfig.FromSubscription(
                subscriptionKey: "d66971f56fcc40f7a25c85ed75a728a9",
                region: "eastus");

            this.speechConfig.SpeechSynthesisVoiceName = "en-US-AriaNeural";
        }

        public async ValueTask<SpeechSynthesisResult> GetSpeechResultAsync(string text)
        {
            using (var speechSynthesizer = new SpeechSynthesizer(speechConfig, null))
            {
                SpeechSynthesisResult speechResult =
                    await speechSynthesizer.SpeakTextAsync(text: text);

                return speechResult;
            }
        }
    }
}

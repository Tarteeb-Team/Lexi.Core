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
                subscriptionKey: "aec1b94cf0254f11b478d28a50743eeb",
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

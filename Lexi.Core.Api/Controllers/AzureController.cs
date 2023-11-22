using Lexi.Core.Api.Services.Azures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AzureController : Controller
    {
        private readonly IAzureService azureService;

        public AzureController(IAzureService azureService)
        {
            this.azureService = azureService;
        }

        [HttpGet]
        public async ValueTask<ActionResult> GetFeedback()
        {
            AudioConfig audioConfig = AudioConfig.FromDefaultMicrophoneInput();

            string result = await this.azureService.TakeFeedbackAsync(audioConfig);

            return Ok(result);
        }
    }
}

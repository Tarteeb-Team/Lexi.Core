using Lexi.Core.Api.Services.Azures;
using Microsoft.AspNetCore.Http;
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

        [HttpPost]
        public async ValueTask<ActionResult> GetFeedback(IFormFile file)
        {
           // AudioConfig audioConfig = AudioConfig.FromWavFileInput(file.FileName);

            string result = await this.azureService.TakeFeedbackAsync(file);

            return Ok(result);
        }
    }
}

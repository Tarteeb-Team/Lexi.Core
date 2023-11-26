//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Models.ObjcetModels;
using Lexi.Core.Api.Services.Orchestrations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CognitiveController : ControllerBase
    {
        private readonly IOrchestrationService orchestrationService;

        public CognitiveController(IOrchestrationService orchestrationService)
        {
            this.orchestrationService = orchestrationService;
        }

        [HttpPost]
        public async Task<string> UploadOggFile(IFormFile formFile)
        {
            using var stream = formFile.OpenReadStream();
            ResponseCognitive responseCognitive = await this.orchestrationService.GetOggFile(stream);

            return responseCognitive.RecognitionStatus;
        }

    }
}

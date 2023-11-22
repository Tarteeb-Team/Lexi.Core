//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using System.IO;
using System.Threading.Tasks;
using Lexi.Core.Api.Services.Orchestrations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            byte[] fileBytes = ConvertFormFileToByteArray(formFile);
            string result = await this.orchestrationService.GetOggFile(fileBytes);
            return result;
        }

        private byte[] ConvertFormFileToByteArray(IFormFile formFile)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                formFile.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}

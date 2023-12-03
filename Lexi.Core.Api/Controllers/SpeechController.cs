//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Models.Foundations.Speeches;
using Lexi.Core.Api.Models.ObjcetModels;
using Lexi.Core.Api.Services.Orchestrations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpeechController : Controller
    {
        private readonly IOrchestrationService orchestrationService;

        public SpeechController(IOrchestrationService orchestrationService)
        {
            this.orchestrationService = orchestrationService;
        }

        //[HttpPost]
        //public async ValueTask<ActionResult<ResponseCognitive>> GetStatus(IFormFile formFile)
        //{
        //    formFile = Request.Form.Files[0];

        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        formFile.CopyTo(memoryStream);
        //        memoryStream.Position = 0;

        //        ResponseCognitive responseCognitive =
        //            await this.orchestrationService.GetOggFile(memoryStream);

        //        return responseCognitive;
        //    }
        //}

        [HttpGet("All")]
        public ActionResult<IQueryable<Speech>> GetAllSpeeches()
        {
            IQueryable<Speech> speeches =
                this.orchestrationService.RetrieveAllSpeeches();

            return Ok(speeches);
        }

        [HttpGet("ById")]
        public async ValueTask<ActionResult<Speech>> GetSpeechByIdAsync(Guid speechId)
        {
            return await this.orchestrationService.RetrieveSpeechByIdAsync(speechId);
        }

        [HttpDelete]
        public async ValueTask<ActionResult<Speech>> DeleteSpeechAsync(Guid speechId)
        {
            Speech speech = await this.orchestrationService.RetrieveSpeechByIdAsync(speechId);

            return await this.orchestrationService.RemoveSpeechAsync(speech);
        }
    }
}

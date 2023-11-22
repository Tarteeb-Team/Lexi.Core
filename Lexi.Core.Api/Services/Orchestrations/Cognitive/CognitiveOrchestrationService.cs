//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System.IO;
using System.Threading.Tasks;
using Lexi.Core.Api.Models.ObjcetModels;
using Lexi.Core.Api.Services.Cognitives;
using Newtonsoft.Json;

namespace Lexi.Core.Api.Services.Orchestrations.Cognitive
{
    public class CognitiveOrchestrationService : ICognitiveOrchestrationService
    {
        private readonly ICognitiveServices cognitiveServices;

        public CognitiveOrchestrationService(ICognitiveServices cognitiveServices)
        {
            this.cognitiveServices = cognitiveServices;
        }

        public async Task<ResponseCognitive> GetOggFile(Stream stream)
        {
            string result = await this.cognitiveServices.GetOggFile(stream);
            ResponseCognitive responseCognitive = new ResponseCognitive();
    
            responseCognitive = JsonConvert.DeserializeObject<ResponseCognitive>(result);
            return responseCognitive; 
        }
    }
}

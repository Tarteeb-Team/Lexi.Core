//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System.IO;
using System.Threading.Tasks;
using Lexi.Core.Api.Services.Cognitives;
using Microsoft.AspNetCore.Http;

namespace Lexi.Core.Api.Services.Orchestrations
{
    public class OrchestrationService : IOrchestrationService
    {
        private readonly ICognitiveServices cogniticeServices;

        public OrchestrationService(ICognitiveServices cogniticeServices)
        {
            this.cogniticeServices = cogniticeServices;
        }

        public async Task<string> GetOggFile(byte[] audio)
        {
            return await this.cogniticeServices.GetOggFile(audio);
        }
    }
}

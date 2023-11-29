//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Brokers.Cognitives;
using System.IO;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Services.Cognitives
{
    public class CognitiveServices : ICognitiveServices
    {
        private readonly ICognitiveBroker cognitiveBroker;

        public CognitiveServices(ICognitiveBroker cognitiveBroker)
        {
            this.cognitiveBroker = cognitiveBroker;
        }

        public async Task<string> GetJsonString()
        {
            return await this.cognitiveBroker.GetJsonString();
        }
    }
}

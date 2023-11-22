//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System.IO;
using System.Threading.Tasks;
using Lexi.Core.Api.Brokers.Cognitives;

namespace Lexi.Core.Api.Services.Cognitives
{
    public class CognitiveServices : ICognitiveServices
    {
        private readonly ICognitiveBroker cognitiveBroker;

        public CognitiveServices(ICognitiveBroker cognitiveBroker)
        {
            this.cognitiveBroker = cognitiveBroker;
        }

        public async Task<string> GetOggFile(byte[] audio)
        {
           return await this.cognitiveBroker.GetOggFile(audio);
        }
    }
}

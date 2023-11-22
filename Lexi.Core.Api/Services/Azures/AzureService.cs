//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Brokers.Azures;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Services.Azures
{
    public class AzureService : IAzureService
    {
        private readonly IAzureBroker azureBroker;
        public async ValueTask<string> TakeFeedbackAsync(AudioConfig audioConfig)
        {
            var result = await this.azureBroker.TakeFeedbackAsync(audioConfig);

            return result;
        }
    }
}

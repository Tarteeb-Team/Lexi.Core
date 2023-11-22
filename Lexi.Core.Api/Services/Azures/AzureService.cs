//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Brokers.Azures;
using Microsoft.AspNetCore.Http;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Services.Azures
{
    public class AzureService : IAzureService
    {
        private readonly IAzureBroker azureBroker;
        public async ValueTask<string> TakeFeedbackAsync(IFormFile formFile)
        {
            string fileName = formFile.FileName;
            var result = await this.azureBroker.TakeFeedbackAsync(fileName);

            return result;
        }
    }
}

//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Microsoft.AspNetCore.Http;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Services.Azures
{
    public interface IAzureService
    {
        ValueTask<string> TakeFeedbackAsync(IFormFile formFile);
    }
}

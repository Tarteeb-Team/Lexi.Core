//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System.Threading.Tasks;
using Lexi.Core.Api.Models.ObjcetModels;

namespace Lexi.Core.Api.Services.Orchestrations.Speech
{
    public interface ISpeechOrchestrationService
    {
        Task MapToFeedback(ResponseCognitive responseCognitive);
        Task MapToSpeech(ResponseCognitive responseCognitive);
    }
}

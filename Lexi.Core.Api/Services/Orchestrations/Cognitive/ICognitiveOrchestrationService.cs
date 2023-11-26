//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Models.ObjcetModels;
using System.IO;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Services.Orchestrations.Cognitive
{
    public interface ICognitiveOrchestrationService
    {
        Task<ResponseCognitive> GetOggFile(Stream stream);
    }
}

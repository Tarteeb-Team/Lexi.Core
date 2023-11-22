//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System.IO;
using System.Threading.Tasks;
using Lexi.Core.Api.Models.ObjcetModels;

namespace Lexi.Core.Api.Services.Orchestrations.Cognitive
{
    public interface ICognitiveOrchestrationService
    {
        Task<ResponseCognitive> GetOggFile(Stream stream);
    }
}

//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System.IO;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Services.Cognitives
{
    public interface ICognitiveServices
    {
        Task<string> GetOggFile();
    }
}

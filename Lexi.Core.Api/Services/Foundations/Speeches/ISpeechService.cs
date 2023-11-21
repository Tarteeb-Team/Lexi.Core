//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System.Threading.Tasks;
using Lexi.Core.Api.Models.Foundations.Speeches;

namespace Lexi.Core.Api.Services.Foundations.Speeches
{
    public interface ISpeechService
    {
        ValueTask<Speech> AddSpechesAsync(Speech speech);
    }
}

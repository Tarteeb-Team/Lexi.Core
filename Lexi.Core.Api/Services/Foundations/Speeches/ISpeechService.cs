//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Models.Foundations.Speeches;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Services.Foundations.Speeches
{
    public interface ISpeechService
    {
        ValueTask<Speech> AddSpechesAsync(Speech speech);
        IQueryable<Speech> RetrieveAllSpeeches();
        ValueTask<Speech> RetrieveSpeechesByIdAsync(Guid id);
        ValueTask<Speech> RemoveSpeechAsync(Speech speech);
    }
}

//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Lexi.Core.Api.Models.Foundations.Speeches;

namespace Lexi.Core.Api.Services.Foundations.Speeches
{
    public interface ISpeechService
    {
        ValueTask<Speech> AddSpechesAsync(Speech speech);
        IQueryable<Speech> RetrieveAllSpeeches();
        ValueTask<Speech> RetrieveSpeechesByIdAsync(Guid id);
    }
}

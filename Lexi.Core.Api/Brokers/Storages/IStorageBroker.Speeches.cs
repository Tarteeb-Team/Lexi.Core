//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Lexi.Core.Api.Models.Foundations.Speeches;

namespace Lexi.Core.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Speech> InsertSpeechAsync(Speech speech);
        IQueryable<Speech> SelectAllSpeeches();
        ValueTask<Speech> SelectSpeechByIdAsync(Guid id);
        ValueTask<Speech> DeleteSpeechAsync(Speech speech);
    }
}

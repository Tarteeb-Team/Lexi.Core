//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Brokers.Loggings;
using Lexi.Core.Api.Brokers.Storages;
using Lexi.Core.Api.Models.Foundations.Speeches;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Services.Foundations.Speeches
{
    public partial class SpeechService : ISpeechService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;

        public SpeechService(IStorageBroker storageBroker, ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Speech> AddSpechesAsync(Speech speech) =>
        TryCatch(async () =>
        {
            ValidateSpeechOnAdd(speech);

            return await this.storageBroker.InsertSpeechAsync(speech);
        });

        public IQueryable<Speech> RetrieveAllSpeeches()
        {
            return this.storageBroker.SelectAllSpeeches();
        }

        public async ValueTask<Speech> RetrieveSpeechesByIdAsync(Guid id)
        {
            return await this.storageBroker.SelectSpeechByIdAsync(id);
        }

        public async ValueTask<Speech> RemoveSpeechAsync(Speech speech)
        {
            return await this.storageBroker.DeleteSpeechAsync(speech);
        }
    }
}

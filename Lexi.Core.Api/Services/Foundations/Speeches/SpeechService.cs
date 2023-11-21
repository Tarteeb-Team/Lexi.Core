//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System.Threading.Tasks;
using Lexi.Core.Api.Brokers.Loggings;
using Lexi.Core.Api.Brokers.Storages;
using Lexi.Core.Api.Models.Foundations.Speeches;

namespace Lexi.Core.Api.Services.Foundations.Speeches
{
    public class SpeechService : ISpeechService
    {
        private readonly IStorageBroker storageBroker;

        public ValueTask<Speech> AddSpechesAsync(Speech speech)
        {
            return this.storageBroker.InsertSpeechAsync(speech);
        }

        public SpeechService(IStorageBroker storageBroker)
        {
            this.storageBroker = storageBroker;
        }

    }
}

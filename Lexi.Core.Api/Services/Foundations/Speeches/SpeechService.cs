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
        private readonly ILoggingBroker loggingBroker;
        
        public SpeechService(IStorageBroker storageBroker, ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
        }
        
        public async ValueTask<Speech> AddSpechesAsync(Speech speech)
        {
            await return this.storageBroker.InsertSpeechAsync(speech);
        }
    }
}

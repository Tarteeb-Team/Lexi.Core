//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Moq;
using Tynamix.ObjectFiller;
using Lexi.Core.Api.Brokers.Loggings;
using Lexi.Core.Api.Brokers.Storages;
using Lexi.Core.Api.Services.Foundations.Speeches;
using SpeechModel = Lexi.Core.Api.Models.Foundations.Speeches.Speech;
using System.Linq.Expressions;
using Xeptions;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Speech
{
    public partial class SpeechServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly ISpeechService speechService;

        public SpeechServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.speechService = new SpeechService(
                storageBroker: this.storageBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static SpeechModel CreateRandomSpeech() =>
            CreateSpeechFiller().Create();
        private Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
         actualException => actualException.SameExceptionAs(expectedException);
        private static Filler<SpeechModel> CreateSpeechFiller() =>
            new Filler<SpeechModel>();
    }
}

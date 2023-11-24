//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Moq;
using Xunit;
using FluentAssertions;
using SpeechModel = Lexi.Core.Api.Models.Foundations.Speeches.Speech;
using Force.DeepCloner;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Speech
{
    public partial class SpeechServiceTests
    {
        [Fact]
        public async Task ShouldAddSpeechAsync()
        {
            //given
            SpeechModel randomSpeech = CreateRandomSpeech();
            SpeechModel inputSpeech = randomSpeech;
            SpeechModel storedSpeech = inputSpeech;
            SpeechModel expectedSpeech = storedSpeech.DeepClone();

            this.storageBrokerMock.Setup(broker =>
            broker.InsertSpeechAsync(inputSpeech))
                .ReturnsAsync(storedSpeech);

            //when
            SpeechModel actualSpeech =
                await this.speechService.AddSpechesAsync(inputSpeech);

            //then
            actualSpeech.Should().BeEquivalentTo(expectedSpeech);

            this.storageBrokerMock.Verify(broker =>
            broker.InsertSpeechAsync(inputSpeech), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

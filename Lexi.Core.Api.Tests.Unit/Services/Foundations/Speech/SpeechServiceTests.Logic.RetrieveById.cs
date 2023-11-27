//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using FluentAssertions;
using Force.DeepCloner;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using SpeechModel = Lexi.Core.Api.Models.Foundations.Speeches.Speech;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Speech
{
    public partial class SpeechServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveSpeechByIdAsync()
        {
            //given
            Guid InputSpeechId = Guid.NewGuid();
            SpeechModel randomSpeech = CreateRandomSpeech();
            SpeechModel storageSpeech = randomSpeech;
            SpeechModel expectedSpeech = storageSpeech.DeepClone();

            this.storageBrokerMock.Setup(broker =>
             broker.SelectSpeechByIdAsync(InputSpeechId)).ReturnsAsync(storageSpeech);
            //when

            SpeechModel actualSpeech =
                await this.speechService.RetrieveSpeechesByIdAsync(InputSpeechId);
            //then
            actualSpeech.Should().BeEquivalentTo(expectedSpeech);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSpeechByIdAsync(InputSpeechId), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using FluentAssertions;
using Lexi.Core.Api.Models.Foundations.Speeches.Exceptions;
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
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            //given
            Guid invalidSpeechId = Guid.Empty;
            var invalidSpeechException = new InvalidSpeechException();

            invalidSpeechException.AddData(
                key: nameof(SpeechModel.Id),
                values: "Id is required");

            var expectedSpeechValidationException =
                    new SpeechValidationException(invalidSpeechException);
            //when
            ValueTask<SpeechModel> retrieveSpeechByIdTask =
                    this.speechService.RetrieveSpeechesByIdAsync(invalidSpeechId);

            SpeechValidationException actualSpeechValidationException =
                await Assert.ThrowsAsync<SpeechValidationException>(retrieveSpeechByIdTask.AsTask);
            //then

            actualSpeechValidationException.Should().BeEquivalentTo(expectedSpeechValidationException);

            this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedSpeechValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThhrowNotFoundExceptionOnRetrieveByIdifSpeechNotFoundAndLogItAsync()
        {
            //given
            Guid sameId = Guid.NewGuid();
            SpeechModel noSpeech = null;

            var notFoundSpeechException =
                    new NotFoundSpeechException(sameId);

            var expectedSpeechValidationException =
                new SpeechValidationException(notFoundSpeechException);

            this.storageBrokerMock.Setup(broker =>
                 broker.SelectSpeechByIdAsync(It.IsAny<Guid>())).ReturnsAsync(noSpeech);

            //when

            ValueTask<SpeechModel> retrieveSpeechTask =
                    this.speechService.RetrieveSpeechesByIdAsync(sameId);

            SpeechValidationException actualSpeechValidationException =
                await Assert.ThrowsAsync<SpeechValidationException>(retrieveSpeechTask.AsTask);

            //then

            actualSpeechValidationException.Should().BeEquivalentTo(expectedSpeechValidationException);

            this.storageBrokerMock.Verify(broker =>
                 broker.SelectSpeechByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(expectedSpeechValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

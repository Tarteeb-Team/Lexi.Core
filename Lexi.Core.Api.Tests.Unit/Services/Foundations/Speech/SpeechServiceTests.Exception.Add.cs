//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using EFxceptions.Models.Exceptions;
using FluentAssertions;
using Lexi.Core.Api.Models.Foundations.Speeches.Exceptions;
using Moq;
using Xunit;
using SpeechModel = Lexi.Core.Api.Models.Foundations.Speeches.Speech;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Speech
{
    public partial class SpeechServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateKeyErrorOccursAndLogItAsync()
        {
            //given
            string someMessage = GetRandomString();
            SpeechModel randomSpeech = CreateRandomSpeech();

            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsSpeechException =
                new AlreadyExistsSpeechException(duplicateKeyException);

            var expectedSpeechDependencyValidationException =
                new SpeechDependencyValidationException(alreadyExistsSpeechException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertSpeechAsync(randomSpeech)).ThrowsAsync(duplicateKeyException);

            //when

            ValueTask<SpeechModel> addSpeechTask = 
                this.speechService.AddSpechesAsync(randomSpeech);

            var actualSpeechDependencyValidationException =
                await Assert.ThrowsAsync<SpeechDependencyValidationException>(addSpeechTask.AsTask);
            //then

            actualSpeechDependencyValidationException.
                Should().
                BeEquivalentTo(expectedSpeechDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                 broker.InsertSpeechAsync(randomSpeech), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
              broker.LogCritical(It.Is(SameExceptionAs(
                  expectedSpeechDependencyValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

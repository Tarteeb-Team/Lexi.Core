//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

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
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsnyc()
        {

            //given
            SpeechModel NoSpeech = null;
            NullSpeechException nullSpeechException = new NullSpeechException();
            SpeechValidationException excpectedSpeechValidationException = 
                    new SpeechValidationException(nullSpeechException);
            //when
            ValueTask<SpeechModel> addSpeechTask = this.speechService.AddSpechesAsync(NoSpeech);

            SpeechValidationException actualSpeechValidationException =
                await Assert.ThrowsAsync<SpeechValidationException>(addSpeechTask.AsTask);
            //then
            actualSpeechValidationException.Should().BeEquivalentTo(excpectedSpeechValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(excpectedSpeechValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
            broker.InsertSpeechAsync(It.IsAny<SpeechModel>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}

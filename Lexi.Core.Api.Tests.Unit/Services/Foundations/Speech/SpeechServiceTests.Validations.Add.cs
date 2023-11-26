//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using FluentAssertions;
using Lexi.Core.Api.Models.Foundations.Speeches.Exceptions;
using Moq;
using System.Threading.Tasks;
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
            var nullSpeechException = new NullSpeechException();
            var excpectedSpeechValidationException =
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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfSpeechIsInvalidLogItAsync(
            string invalidText)
        {
            // given
            var invalidSpeech = new SpeechModel
            {
                Sentence = invalidText
            };

            var invalidSpeechException = new InvalidSpeechException();

            invalidSpeechException.AddData(
                key: nameof(SpeechModel.Id),
                values: "Id is required");

            invalidSpeechException.AddData(
                key: nameof(SpeechModel.Sentence),
                values: "Text is required");

            invalidSpeechException.AddData(
                key: nameof(SpeechModel.UserId),
                values: "Id is required");

            invalidSpeechException.AddData(
                key: nameof(SpeechModel.User),
                values: "User is required");

            invalidSpeechException.AddData(
                key: nameof(SpeechModel.Feedbacks),
                values: "Feedback is required");

            var expectedSpeechValidationException =
                new SpeechValidationException(invalidSpeechException);

            // when
            ValueTask<SpeechModel> addSpeechTask =
                this.speechService.AddSpechesAsync(invalidSpeech);

            var actualSpeechValidationException =
                await Assert.ThrowsAsync<SpeechValidationException>(addSpeechTask.AsTask);

            // then
            actualSpeechValidationException.Should()
                .BeEquivalentTo(expectedSpeechValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    actualSpeechValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
            broker.InsertSpeechAsync(It.IsAny<SpeechModel>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

    }
}

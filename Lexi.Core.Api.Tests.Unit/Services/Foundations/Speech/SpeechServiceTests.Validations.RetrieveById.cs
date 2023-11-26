//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}

//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using EFxceptions.Models.Exceptions;
using FluentAssertions;
using Lexi.Core.Api.Models.Foundations.Speeches.Exceptions;
using Microsoft.Data.SqlClient;
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
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateKeyErrorOccursAndLogItAsync()
        {
            //given
            string someMessage = GetRandomString();
            SpeechModel randomSpeech = CreateRandomSpeech();

            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsSpeechException =
                new AlreadyExistValidationException(duplicateKeyException);

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
              broker.LogError(It.Is(SameExceptionAs(
                  expectedSpeechDependencyValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptiononAddIfSqlErrorOccursAndLogItAsync()
        {
            //given
            SpeechModel randomSpeech = CreateRandomSpeech();
            SqlException sqlException = GetSqlError();
            var failedSpeechStorageException = new FailedSpeechStorageException(sqlException);

            var expectedSpeechDependencyExcpetion =
                new SpeechDependencyException(failedSpeechStorageException);

            this.storageBrokerMock.Setup(broker =>
            broker.InsertSpeechAsync(randomSpeech))
                .ThrowsAsync(sqlException);

            //when
            ValueTask<SpeechModel> addSpeechTask =
                this.speechService.AddSpechesAsync(randomSpeech);

            //then
            await Assert.ThrowsAsync<SpeechDependencyException>(() =>
                addSpeechTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSpeechAsync(randomSpeech),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedSpeechDependencyExcpetion))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccursAndLogItAsync()
        {
            //given
            SpeechModel randomSpeech = CreateRandomSpeech();
            var serviceException = new Exception();

            var failedSpeechServiceException =
                new FailedSpeechServiceException(serviceException);

            var expectedSpeechServiceException =
                new SpeechServiceException(failedSpeechServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertSpeechAsync(randomSpeech))
                .ThrowsAsync(serviceException);

            //when
            ValueTask<SpeechModel> addSpeechTask =
                this.speechService.AddSpechesAsync(randomSpeech);

            //then
            await Assert.ThrowsAsync<SpeechServiceException>(() =>
                addSpeechTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSpeechAsync(randomSpeech),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(expectedSpeechServiceException))),
                    Times.Once);
        }
    }
}
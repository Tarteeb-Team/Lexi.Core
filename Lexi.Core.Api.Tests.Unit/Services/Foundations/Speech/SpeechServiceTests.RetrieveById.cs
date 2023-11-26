//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Lexi.Core.Api.Models.Foundations.Speeches.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;
using SpeechModel = Lexi.Core.Api.Models.Foundations.Speeches.Speech;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Speech
{
    public partial class SpeechServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdAsyncIfSqlErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = GetSqlError();

            var failedSpeechStorageException =
                    new FailedSpeechStorageException(sqlException);

            SpeechDependencyException expectedSpeechDependencyException =
                    new SpeechDependencyException(failedSpeechStorageException);

            this.storageBrokerMock.Setup(broker =>
            broker.SelectSpeechByIdAsync(It.IsAny<Guid>())).
                ThrowsAsync(sqlException);
            //when
            ValueTask<SpeechModel> retrieveSpeechByIdTask =
                    this.speechService.RetrieveSpeechesByIdAsync(someId);

            SpeechDependencyException actualSpeechDependencyException =
                await Assert.ThrowsAsync<SpeechDependencyException>
                                (retrieveSpeechByIdTask.AsTask);
            //then
            actualSpeechDependencyException.
                Should().
                BeEquivalentTo(expectedSpeechDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSpeechByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(
                expectedSpeechDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdAsyncIfServiceErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            Exception serviceException = new Exception();

            var failedSpeechServiceException = 
                    new FailedSpeechServiceException(serviceException);

            var expectedspeechServiceException = new 
                    SpeechServiceException(failedSpeechServiceException);

            this.storageBrokerMock.Setup(broker => 
                 broker.SelectSpeechByIdAsync(It.IsAny<Guid>())).ThrowsAsync(serviceException);
            //when
            ValueTask<SpeechModel> retrieveSpeechByIdTask =
                 this.speechService.RetrieveSpeechesByIdAsync(someId);

            SpeechServiceException actualSpeechServiceException =
                await Assert.ThrowsAsync<SpeechServiceException>(
                    retrieveSpeechByIdTask.AsTask);
            //then
            actualSpeechServiceException.
                Should().
                BeEquivalentTo(expectedspeechServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSpeechByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                expectedspeechServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

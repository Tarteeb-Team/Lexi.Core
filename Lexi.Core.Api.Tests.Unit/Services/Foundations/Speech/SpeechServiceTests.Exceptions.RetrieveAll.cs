//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using FluentAssertions;
using Lexi.Core.Api.Models.Foundations.Speeches.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Speech
{
    public partial class SpeechServiceTests
    {
        [Fact]
        public void ShoulThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {   //given
            SqlException sqlException = GetSqlError();

            var failedSpeechServiceException =
                new FailedSpeechServiceException(sqlException);

            var expectedSpeechDependencyException =
                new SpeechDependencyException(failedSpeechServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllSpeeches()).Throws(sqlException);
            //when

            Action retrieveAllSpeechAction = () =>
                    this.speechService.RetrieveAllSpeeches();

            SpeechDependencyException actualSpeechDependencyException =
                Assert.Throws<SpeechDependencyException>(retrieveAllSpeechAction);

            //then
            actualSpeechDependencyException.
                Should().
                BeEquivalentTo(expectedSpeechDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllSpeeches(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(
                expectedSpeechDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowCriticalServiceExceptionOnRetrieveAllWhenServiceErrorOccursAndLogIt()
        {
            //given
            Exception serviceException = new Exception();

            FailedSpeechServiceException failedSpeechServiceException =
                new FailedSpeechServiceException(serviceException);

            SpeechServiceException expectedSpeechServiceException =
                    new SpeechServiceException(failedSpeechServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllSpeeches()).Throws(serviceException);
            //when
            Action actualSpeeches = () =>
                    this.speechService.RetrieveAllSpeeches();

            SpeechServiceException actualSpeechServiceException =
                Assert.Throws<SpeechServiceException>(actualSpeeches);

            //then
            actualSpeechServiceException.
                Should().
                BeEquivalentTo(expectedSpeechServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllSpeeches(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(
                expectedSpeechServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

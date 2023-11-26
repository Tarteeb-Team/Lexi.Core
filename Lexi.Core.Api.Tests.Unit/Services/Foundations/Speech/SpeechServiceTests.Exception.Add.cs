//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using FluentAssertions;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Models.Foundations.Feedbacks.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Feedbacks
{
    public partial class FeedbackServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            //given
            Guid someFeedbackId = Guid.NewGuid();
            SqlException sqlException = GetSqlError();

            var failedFeedbackStorageException =
                new FailedFeedbackStorageException(sqlException);

            var expectedFeedbackDependencyException =
                new FeedbackDependencyException(failedFeedbackStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectFeedbackByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);
            //when
            ValueTask<Feedback> retrieveFeedbackById =
                this.feedbackService.RetrieveFeedbackByIdAsync(someFeedbackId);

            FeedbackDependencyException actualFeedbackDependencyException =
                await Assert.ThrowsAsync<FeedbackDependencyException>(
                    retrieveFeedbackById.AsTask);

            //then
            actualFeedbackDependencyException
                .Should().BeEquivalentTo(expectedFeedbackDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectFeedbackByIdAsync(someFeedbackId), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedFeedbackDependencyException))), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdAsyncIfServiceErrorOccursAndLogItAsync()
        {
            //given
            Guid someFeedbackId = Guid.NewGuid();
            Exception serverException = new Exception();

            var failedFeedbackServiceException =
                new FailedFeedbackServiceException(serverException);

            var expectedFeedbackServiceException =
                new FeedbackServiceException(failedFeedbackServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectFeedbackByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(serverException);

            //when
            ValueTask<Feedback> retrieveFeedbackById =
                this.feedbackService.RetrieveFeedbackByIdAsync(someFeedbackId);

            FeedbackServiceException actualFeedbackServiceException =
                await Assert.ThrowsAsync<FeedbackServiceException>(retrieveFeedbackById.AsTask);

            //then
            actualFeedbackServiceException
                .Should().BeEquivalentTo(expectedFeedbackServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectFeedbackByIdAsync(someFeedbackId), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedFeedbackServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

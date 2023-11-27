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
using System.Linq;
using Xunit;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Feedbacks
{
    public partial class FeedbackServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveIfSqlErrorOccuredAndLogIt()
        {
            //given
            IQueryable<Feedback> someFeedbacks = CreateRandomFeedbacks();
            SqlException sqlException = GetSqlError();
            var failedFeedbackStorageException = new FailedFeedbackStorageException(sqlException);

            var expectedFeedbackDependencyException =
                new FeedbackDependencyException(failedFeedbackStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllFeedbacks())
                .Throws(sqlException);

            //when
            Action retrieveAllFeedbacksAction = () =>
                this.feedbackService.RetrieveAllFeedbacks();

            FeedbackDependencyException feedbackDependencyException =
                Assert.Throws<FeedbackDependencyException>(
                retrieveAllFeedbacksAction);

            feedbackDependencyException.Should().BeEquivalentTo(expectedFeedbackDependencyException);

            //then
            this.storageBrokerMock.Verify(broker => broker.SelectAllFeedbacks(),
                Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(expectedFeedbackDependencyException))),
                Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }


        [Fact]
        public void ShouldThrowSeviceExceptionOnRetrieveIfErrorOccursAndLogIt()
        {
            //given
            IQueryable<Feedback> someFeedbacks = CreateRandomFeedbacks();
            var serviceException = new Exception();

            var failedFeedbackServiceException =
                new FailedFeedbackServiceException(serviceException);

            var expectedFeedbackServiceException =
                new FeedbackServiceException(failedFeedbackServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllFeedbacks())
                .Throws(serviceException);

            //when
            Action retrieveAllFeedbackAction = () =>
                this.feedbackService.RetrieveAllFeedbacks();

            FeedbackServiceException feedbackServiceException =
                Assert.Throws<FeedbackServiceException>(retrieveAllFeedbackAction);

            feedbackServiceException.Should().BeEquivalentTo(expectedFeedbackServiceException);

            //then
            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllFeedbacks(),
                Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedFeedbackServiceException))),
                Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();

        }
    }
}

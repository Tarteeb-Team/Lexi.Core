//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Models.Foundations.Feedbacks.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;
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
                .Returns(someFeedbacks);

            //when
            Action retrieveAllFeedbacksAction = () =>
                this.feedbackService.RetrieveAllFeedbacks();

            FeedbackDependencyException feedbackDependencyException =
                Assert.Throws<FeedbackDependencyException>(() =>
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
    }
}

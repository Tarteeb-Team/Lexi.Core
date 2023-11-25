//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            //given
            Feedback someFeedback = CreateRandomFeedback();
            SqlException sqlException = GetSqlError();
            var failedFeedbackStorageException = new FailedFeedbackStorageException(sqlException);

            var expectedFeedbackDependencyException =
                new FeedbackDependencyException(failedFeedbackStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertFeedbackAsync(someFeedback))
                .ThrowsAsync(sqlException);

            //when
            ValueTask<Feedback> addFeedbackTask =
                this.feedbackService.AddFeedbackAsync(someFeedback);

            FeedbackDependencyException feedbackDependencyException =
                await Assert.ThrowsAsync<FeedbackDependencyException>(()  =>
                    addFeedbackTask.AsTask());

            feedbackDependencyException.Should().BeEquivalentTo(expectedFeedbackDependencyException);

            //then

            this.storageBrokerMock.Verify(broker =>
                broker.InsertFeedbackAsync(someFeedback), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(feedbackDependencyException))),
                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfAlreadyExistErrorOccursAndLogItAsync()
        {
            //given
            Feedback someFeedback = CreateRandomFeedback();
            var duplicateKeyException = new DuplicateKeyException(GetRandomString());

            var alreadyExistValidationException =
                new AlreadyExistValidationException(duplicateKeyException);

            var expectedFeedbackDependencyValidationException =
                new FeedbackDependencyValidationException(alreadyExistValidationException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertFeedbackAsync(someFeedback))
                .ThrowsAsync(duplicateKeyException);

            //when
            ValueTask<Feedback> addFeedbackTask =
                this.feedbackService.AddFeedbackAsync(someFeedback);

            FeedbackDependencyValidationException feedbackDependencyValidationException =
                await Assert.ThrowsAsync<FeedbackDependencyValidationException>(() =>
                    addFeedbackTask.AsTask());

            feedbackDependencyValidationException.Should().BeEquivalentTo(expectedFeedbackDependencyValidationException);

            //then

            this.storageBrokerMock.Verify(broker =>
                broker.InsertFeedbackAsync(someFeedback), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedFeedbackDependencyValidationException))),
                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

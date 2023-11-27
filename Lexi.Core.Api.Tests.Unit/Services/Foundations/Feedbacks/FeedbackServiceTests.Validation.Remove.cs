//===========================
// Copyright (c) Tarteeb LLC
// Manage Your Money Easy
//===========================

using FluentAssertions;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Models.Foundations.Feedbacks.Exceptions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Feedbacks
{
    public partial class FeedbackServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            // given 
            Guid invalidFeedbackId = Guid.Empty;

            var invalidFeedbackException =
                new InvalidFeedbackException();

            invalidFeedbackException.AddData(
                key: nameof(Feedback.Id),
                values: "Id is required");

            var expectedFeedbackValidationException =
                new FeedbackValidationException(invalidFeedbackException);

            // when
            ValueTask<Feedback> removeFeedbackById =
                this.feedbackService.RemoveFeedbackAsync(invalidFeedbackId);

            FeedbackValidationException actualFeedbackValidationException =
                await Assert.ThrowsAsync<FeedbackValidationException>(
                    removeFeedbackById.AsTask);
            // then
            actualFeedbackValidationException.Should()
                .BeEquivalentTo(expectedFeedbackValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedFeedbackValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectFeedbackByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.storageBrokerMock.Verify(broker =>
            broker.DeleteFeedbackAsync(It.IsAny<Feedback>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRemoveFeedbackByIdIsNotFoundAndLogItAsync()
        {
            // given
            Guid inputFeedbackId = Guid.NewGuid();
            Feedback noFeedback = null;

            var notFoundFeedbackException =
                new NotFoundFeedbackException(inputFeedbackId);

            var expectedFeedbackValidationException =
                new FeedbackValidationException(notFoundFeedbackException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectFeedbackByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(noFeedback);

            // when
            ValueTask<Feedback> removeFeedbackById =
                this.feedbackService.RemoveFeedbackAsync(inputFeedbackId);

            var actualFeedbackValidationException =
                await Assert.ThrowsAsync<FeedbackValidationException>(
                    removeFeedbackById.AsTask);

            // then
            actualFeedbackValidationException.Should().BeEquivalentTo(expectedFeedbackValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectFeedbackByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedFeedbackValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteFeedbackAsync(It.IsAny<Feedback>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

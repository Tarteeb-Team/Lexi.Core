//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using FluentAssertions;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Models.Foundations.Feedbacks.Exceptions;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Feedbacks
{
    public partial class FeedbackServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddOfInputIsNullAndLogItAsync()
        {
            //given
            Feedback nullFeedback = null;
            var nullFeedbackException = new NullFeedbackException();

            var expectedfeedbackValidationException =
                new FeedbackValidationException(nullFeedbackException);

            //when
            ValueTask<Feedback> addFeedbackTask = this.feedbackService.AddFeedbackAsync(nullFeedback);

            FeedbackValidationException actualFeedbackValidationException =
                await Assert.ThrowsAsync<FeedbackValidationException>(addFeedbackTask.AsTask);

            //then
            actualFeedbackValidationException
                .Should().BeEquivalentTo(expectedfeedbackValidationException);

            this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedfeedbackValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertFeedbackAsync(It.IsAny<Feedback>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData]
        public async Task ShouldThrowValidationExceptionOnAddIfFeedbackIsInvalidAndLogItAsync(Guid invalidId = default)
        {
            //given
            Feedback invalidFeedback = new Feedback
            {
                Id = invalidId
            };

            var invalidFeedbackException = new InvalidFeedbackException();

            invalidFeedbackException.AddData(
                key: nameof(Feedback.Id),
                values: "Id is required");

            var expectedFeedbackValidationException =
                new FeedbackValidationException(invalidFeedbackException);

            //when
            ValueTask<Feedback> addFeedbackTask =
                this.feedbackService.AddFeedbackAsync(invalidFeedback);

            FeedbackValidationException actualFeedbackValidationException =
                await Assert.ThrowsAsync<FeedbackValidationException>(addFeedbackTask.AsTask);

            //then
            actualFeedbackValidationException.Should().BeEquivalentTo(expectedFeedbackValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedFeedbackValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertFeedbackAsync(It.IsAny<Feedback>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}

﻿//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System.Threading.Tasks;
using FluentAssertions;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Models.Foundations.Feedbacks.Exceptions;
using Moq;
using Xunit;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Feedbacks
{
    public partial class FeedbackServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveIfFeedbackIsNullAndLogItAsync()
        {
            //given
            Feedback nullFeedback = null;
            var nullFeedbackException = new NullFeedbackException();

            FeedbackValidationException expectedFeedbackValidationException =
                new FeedbackValidationException(nullFeedbackException);

            //when
            ValueTask<Feedback> removeFeedbackTask =
                this.feedbackService.RemoveFeedbackAsync(nullFeedback);

            FeedbackValidationException feedbackValidationException =
                await Assert.ThrowsAsync<FeedbackValidationException>(() =>
                removeFeedbackTask.AsTask());

            feedbackValidationException.Should().BeEquivalentTo(expectedFeedbackValidationException);

            //then
            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(feedbackValidationException))),
                Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteFeedbackAsync(It.IsAny<Feedback>()),
                Times.Never());

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}

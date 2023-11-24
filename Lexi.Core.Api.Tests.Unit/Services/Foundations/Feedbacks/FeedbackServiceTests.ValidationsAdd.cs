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
    }
}

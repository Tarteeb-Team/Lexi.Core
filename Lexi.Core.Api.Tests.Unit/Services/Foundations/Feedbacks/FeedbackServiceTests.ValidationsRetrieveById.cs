//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

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
        public async Task ShouldThrowValidationExceptionOnRetriveByIdIsInvalidAndLogItAsync()
        {
            //given
            Guid invalidFeedackId = Guid.Empty;
            var invalidFeedbackException = new InvalidFeedbackException();

            invalidFeedbackException.AddData(
                key: nameof(Feedback.Id),
                values: "Id is required");

            var expectedFeedbackValidationException =
                new FeedbackValidationException(invalidFeedbackException);

            //when
            ValueTask<Feedback> retrieveFeedbackById =
                this.feedbackService.RetrieveFeedbackByIdAsync(invalidFeedackId);

            FeedbackValidationException actualFeedbackValidationException =
                await Assert.ThrowsAsync<FeedbackValidationException>(retrieveFeedbackById.AsTask);

            //then
            actualFeedbackValidationException
                .Should().BeEquivalentTo(expectedFeedbackValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedFeedbackValidationException))), Times.Once());

            this.storageBrokerMock.Verify(broker =>
            broker.SelectFeedbackByIdAsync(It.IsAny<Guid>()), Times.Never());

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}

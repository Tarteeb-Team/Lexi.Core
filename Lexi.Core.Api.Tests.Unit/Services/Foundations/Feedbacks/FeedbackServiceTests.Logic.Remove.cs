//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Moq;
using Xunit;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Feedbacks
{
    public partial class FeedbackServiceTests
    {
        [Fact]
        public async Task ShouldRemoveFeedbackAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputFeedbackId = randomId;
            Feedback randomFeedback = CreateRandomFeedback();
            Feedback storageFeedback = randomFeedback;
            Feedback expectedInputFeedback = storageFeedback;
            Feedback deletedFeedback = expectedInputFeedback;
            Feedback expectedFeedback = deletedFeedback.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectFeedbackByIdAsync(inputFeedbackId))
                    .ReturnsAsync(storageFeedback);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteFeedbackAsync(expectedInputFeedback))
                    .ReturnsAsync(deletedFeedback);

            // when
            Feedback actualFeedback = await this.feedbackService
                .RemoveFeedbackAsync(inputFeedbackId);

            // then
            actualFeedback.Should().BeEquivalentTo(expectedFeedback);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectFeedbackByIdAsync(inputFeedbackId),
                    Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteFeedbackAsync(expectedInputFeedback),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();

        }
    }
}

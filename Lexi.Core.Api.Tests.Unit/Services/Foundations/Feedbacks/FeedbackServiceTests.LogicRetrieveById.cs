//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using FluentAssertions;
using Force.DeepCloner;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Feedbacks
{
    public partial class FeedbackServiceTests
    {
        [Fact]
        public async Task ShoulRetrieveFeedbackByIdAsync()
        {
            //given
            Guid randomFeedbackId = Guid.NewGuid();
            Guid inputFeedbackId = randomFeedbackId;
            Feedback randomFeedback = CreateRandomFeedback();
            Feedback persistedFeedback = randomFeedback;
            Feedback expectedFeedback = persistedFeedback.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectFeedbackByIdAsync(inputFeedbackId))
                    .ReturnsAsync(persistedFeedback);

            //when
            Feedback actualFeedback =
                await this.feedbackService.RetrieveFeedbackByIdAsync(inputFeedbackId);

            //then
            actualFeedback.Should().BeEquivalentTo(expectedFeedback);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectFeedbackByIdAsync(inputFeedbackId), Times.Exactly(2));

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

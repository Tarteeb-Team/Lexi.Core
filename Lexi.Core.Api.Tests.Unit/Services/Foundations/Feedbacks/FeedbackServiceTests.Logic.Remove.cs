//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

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
            //given
            Feedback randomFeedback = CreateRandomFeedback();
            Feedback inputFeedback = randomFeedback.DeepClone();
            Feedback deletedFeedback = randomFeedback.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteFeedbackAsync(inputFeedback))
                .ReturnsAsync(deletedFeedback);

            //when
            Feedback actualFeedback =
                await this.feedbackService.RemoveFeedbackAsync(inputFeedback);

            actualFeedback.Should().BeEquivalentTo(deletedFeedback);

            //then
            this.storageBrokerMock.Verify(broker =>
                broker.DeleteFeedbackAsync(inputFeedback),Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();

        }
    }
}

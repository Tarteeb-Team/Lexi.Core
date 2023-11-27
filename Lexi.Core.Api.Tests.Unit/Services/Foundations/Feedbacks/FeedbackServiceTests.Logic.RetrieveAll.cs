//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using FluentAssertions;
using Force.DeepCloner;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Feedbacks
{
    public partial class FeedbackServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllFeedbacks()
        {
            //given
            IQueryable<Feedback> feedbacks = CreateRandomFeedbacks();
            IQueryable<Feedback> expectedFeedbacks = feedbacks.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllFeedbacks())
                .Returns(feedbacks);

            //when
            this.feedbackService.RetrieveAllFeedbacks();

            //then
            feedbacks.Should().BeEquivalentTo(expectedFeedbacks);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllFeedbacks(),
                Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

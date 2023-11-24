//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using FluentAssertions;
using Force.DeepCloner;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Feedbacks
{
    public partial class FeedbackServiceTests
    {
        [Fact]
        public async Task ShouldAddFeedbackAsycn()
        {
            //given
            Feedback randomFeedback = CreateRandomFeedback();
            Feedback inputFeedback = randomFeedback;
            Feedback persistedFeedback = inputFeedback;
            Feedback excpectedFeedback = persistedFeedback.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.InsertFeedbackAsync(inputFeedback))
                    .ReturnsAsync(excpectedFeedback);
             //when 
             Feedback actualFeedback = 
                await this.feedbackService.AddFeedbackAsync(inputFeedback);

            //then
            actualFeedback.Should().BeEquivalentTo(excpectedFeedback);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertFeedbackAsync(inputFeedback), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}

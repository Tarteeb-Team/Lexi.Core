//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using FluentAssertions;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Models.Foundations.Feedbacks.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Feedbacks
{
    public partial class FeedbackServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someFeedbackId = Guid.NewGuid();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedFeedbackException =
                new LockedFeedbackException(dbUpdateConcurrencyException);

            var expectedFeedbackDependencyValidationException =
                new FeedbackDependencyValidationException(lockedFeedbackException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectFeedbackByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<Feedback> removeFeedbackById =
                this.feedbackService.RemoveFeedbackAsync(someFeedbackId);

            var actualFeedbackDependencyValidationException =
                await Assert.ThrowsAsync<FeedbackDependencyValidationException>(
                    removeFeedbackById.AsTask);

            // then
            actualFeedbackDependencyValidationException.Should()
                .BeEquivalentTo(expectedFeedbackDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectFeedbackByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedFeedbackDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteFeedbackAsync(It.IsAny<Feedback>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

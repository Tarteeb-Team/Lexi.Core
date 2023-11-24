//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Models.Foundations.Feedbacks.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading.Tasks;
using Xeptions;

namespace Lexi.Core.Api.Services.Foundations.Feedbacks
{
    public partial class FeedbackService
    {
        private delegate ValueTask<Feedback> ReturningFeedbackFunction();

        private async ValueTask<Feedback> TryCatch(ReturningFeedbackFunction returningFeedbackFunction)
        {
            try
            {
                return await returningFeedbackFunction();
            }
            catch(NullFeedbackException nullFeedbackException)
            {
                throw CreateAndLogValidationException(nullFeedbackException);
            }
        }

        private FeedbackValidationException CreateAndLogValidationException(Xeption xeption)
        {
            var feedbackValidationException = new FeedbackValidationException(xeption);
            this.loggingBroker.LogError(feedbackValidationException);

            return feedbackValidationException;
        }
    }
}

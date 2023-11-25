﻿//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Models.Foundations.Feedbacks.Exceptions;
using Microsoft.Data.SqlClient;
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
            catch(InvalidFeedbackException invalidFeedbackException)
            {
                throw CreateAndLogValidationException(invalidFeedbackException);
            }
            catch(SqlException sqlException)
            {
                var failedFeedbackStorageException = new FailedFeedbackStorageException(sqlException);

                throw CreateAndLogCriticalDepenedencyException(failedFeedbackStorageException);
            }
        }

        private FeedbackValidationException CreateAndLogValidationException(Xeption xeption)
        {
            var feedbackValidationException = new FeedbackValidationException(xeption);
            this.loggingBroker.LogError(feedbackValidationException);

            return feedbackValidationException;
        }

        private FeedbackDependencyException CreateAndLogCriticalDepenedencyException(Xeption exception)
        {
            var feedbackDependencyException = new FeedbackDependencyException(exception);

            this.loggingBroker.LogCritical(feedbackDependencyException);

            return feedbackDependencyException;
        }
    }
}

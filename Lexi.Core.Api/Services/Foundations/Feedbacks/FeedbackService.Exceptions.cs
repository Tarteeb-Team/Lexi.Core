//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using EFxceptions.Models.Exceptions;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Models.Foundations.Feedbacks.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xeptions;

namespace Lexi.Core.Api.Services.Foundations.Feedbacks
{
    public partial class FeedbackService
    {
        private delegate ValueTask<Feedback> ReturningFeedbackFunction();
        private delegate IQueryable<Feedback> ReturningFeedbacksFunction();

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
            catch(DuplicateKeyException duplicateKeyException)
            {
                AlreadyExistValidationException alreadyExistValidationException =
                    new AlreadyExistValidationException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistValidationException);
            }
            catch(Exception exception)
            {
                var feedbackServiceException = new FailedFeedbackServiceException(exception);

                throw CreateAndLogServiceException(feedbackServiceException);
            }
        }

        private IQueryable<Feedback> TryCatch(ReturningFeedbacksFunction returningFeedbacksFunction)
        {
            try
            {
                return returningFeedbacksFunction();
            }
            catch (SqlException sqlException)
            {
                var failedFeedbackStorageException =
                    new FailedFeedbackStorageException(sqlException);

                throw CreateAndLogCriticalDepenedencyException(failedFeedbackStorageException);
            }
            catch(Exception exception)
            {
                var failedFeedbackServiceException =
                    new FailedFeedbackServiceException(exception);

                throw CreateAndLogServiceException(failedFeedbackServiceException);
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

        private FeedbackDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var feedbackDependencyValidationException =
                new FeedbackDependencyValidationException(exception);

            this.loggingBroker.LogError(feedbackDependencyValidationException);

            return feedbackDependencyValidationException;
        }

        private FeedbackServiceException CreateAndLogServiceException(Xeption exception)
        {
            var feedbackServiceException = new FeedbackServiceException(exception);

            this.loggingBroker.LogError(feedbackServiceException);
            
            return feedbackServiceException;
        }
    }
}

//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Models.Foundations.Feedbacks.Exceptions;
using System;

namespace Lexi.Core.Api.Services.Foundations.Feedbacks
{
    public partial class FeedbackService
    {
        private void VaidateFeedbackOnAdd(Feedback feedback)
        {
            ValidateFeedbackNotNull(feedback);

            Validate(
                (Rule: IsInvalid(feedback.Id), Parameter: nameof(Feedback.Id)));
        }

        private void VaidateFeedbackOnRemove(Feedback feedback)
        {
            ValidateFeedbackNotNull(feedback);

            Validate(
                (Rule: IsInvalid(feedback.Id), Parameter: nameof(Feedback.Id)));
        }

        private static dynamic IsInvalid(Guid feeedbakcId) => new
        {
            Condition = feeedbakcId == default,
            Message = "Id is required"
        };

        private void ValidateFeedbackNotNull(Feedback feedback)
        {
            if (feedback == null)
            {
                throw new NullFeedbackException();
            }
        }

        private static void ValidateFeedbackId(Guid feedbackId) =>
            Validate((Rule: IsInvalid(feedbackId), Parameter: nameof(Feedback.Id)));

        private static void ValidateStorageFeedback(Feedback maybeFeedback, Guid feedbackId)
        {
            if (maybeFeedback == null)
            {
                throw new NotFoundFeedbackException(feedbackId);
            }
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidFeedbackException = new InvalidFeedbackException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidFeedbackException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidFeedbackException.ThrowIfContainsErrors();
        }
    }
}

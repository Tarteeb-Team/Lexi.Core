//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Feedbacks.Exceptions
{
    public class NotFoundFeedbackException : Xeption
    {
        public NotFoundFeedbackException(Guid feedbackId)
            : base(message: $"Feedback not found with id: {feedbackId}.")
        { }
    }
}

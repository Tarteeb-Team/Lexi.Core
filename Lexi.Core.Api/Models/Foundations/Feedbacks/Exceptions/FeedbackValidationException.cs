//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Feedbacks.Exceptions
{
    public class FeedbackValidationException : Xeption
    {
        public FeedbackValidationException(Xeption xeption)
            : base(message: "Feedback validation error occurred, fix the error and try again.")
        { }
    }
}

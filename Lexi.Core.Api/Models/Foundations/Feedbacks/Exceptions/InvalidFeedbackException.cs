//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Feedbacks.Exceptions
{
    public class InvalidFeedbackException : Xeption
    {
        public InvalidFeedbackException()
            :base(message: "Feedback is invalid.")
        { }
    }
}

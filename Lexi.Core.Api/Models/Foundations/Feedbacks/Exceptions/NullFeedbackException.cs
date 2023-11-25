//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Feedbacks.Exceptions
{
    public class NullFeedbackException : Xeption
    {
        public NullFeedbackException()
            :base(message: "Feedback is null.")
        { }
    }
}

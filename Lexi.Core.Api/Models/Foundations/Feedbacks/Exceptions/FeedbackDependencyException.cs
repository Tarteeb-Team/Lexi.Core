//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Feedbacks.Exceptions
{
    public class FeedbackDependencyException : Xeption
    {
        public FeedbackDependencyException(Xeption innerException)
            :base("Feedback dependency error occured, contact support",
                 innerException)
        { }
    }
}

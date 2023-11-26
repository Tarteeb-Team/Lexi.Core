//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Feedbacks.Exceptions
{
    public class FeedbackServiceException : Xeption
    {
        public FeedbackServiceException(Xeption innerException)
            : base("Feedback service error occured, contact support",
                 innerException)
        { }
    }
}

//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Feedbacks.Exceptions
{
    public class FeedbackDependencyValidationException : Xeption
    {
        public FeedbackDependencyValidationException(Xeption innerException)
            : base("Dependency validation error occured, fix the error and try again",
                 innerException)
        { }
    }
}

//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Feedbacks.Exceptions
{
    public class LockedFeedbackException : Xeption
    {
        public LockedFeedbackException(Exception innerException)
            : base(message: "Feedback is locked, please try again.", innerException)
        { }
    }
}

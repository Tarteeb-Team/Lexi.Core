//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Feedbacks.Exceptions
{
    public class FailedFeedbackServiceException : Xeption
    {
        public FailedFeedbackServiceException(Exception innerException)
            :base("Failed feedback service error occured, contact support",
                 innerException)
        { }
    }
}

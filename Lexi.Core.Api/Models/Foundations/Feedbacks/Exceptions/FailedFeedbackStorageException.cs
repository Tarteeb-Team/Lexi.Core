//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Feedbacks.Exceptions
{
    public class FailedFeedbackStorageException : Xeption
    {
        public FailedFeedbackStorageException(Exception innerExxception)
            : base("Failed feedback storage error occured, contact support",
                 innerExxception)
        { }
    }
}

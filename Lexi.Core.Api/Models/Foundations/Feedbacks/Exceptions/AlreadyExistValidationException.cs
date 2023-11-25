//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Feedbacks.Exceptions
{
    public class AlreadyExistValidationException : Xeption
    {
        public AlreadyExistValidationException(Exception innerException)
            :base("Alread exist validation error occured,fix the error and try again",
                 innerException)
        { }
    }
}

//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Speeches.Exceptions
{
    public class AlreadyExistValidationException : Xeption
    {
        public AlreadyExistValidationException(Exception exception)
            : base(message: "Speech already exists", exception)
        { }
    }
}

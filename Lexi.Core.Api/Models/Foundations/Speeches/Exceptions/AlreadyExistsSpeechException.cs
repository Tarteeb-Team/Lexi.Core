//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Speeches.Exceptions
{
    public class AlreadyExistsSpeechException : Xeption
    {
        public AlreadyExistsSpeechException(Exception exception)
            : base(message: "Speech already exists", exception)
        { }
    }
}

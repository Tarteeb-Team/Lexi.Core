//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Speeches.Exceptions
{
    public class SpeechValidationException : Xeption
    {
        public SpeechValidationException(Xeption innerException)
            : base(message: "Speech validation error occurred, fix the errors and try again ", innerException)
        { }
    }
}

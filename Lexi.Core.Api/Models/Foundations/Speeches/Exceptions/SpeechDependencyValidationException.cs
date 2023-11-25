//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Speeches.Exceptions
{
    public class SpeechDependencyValidationException : Xeption
    {
        public SpeechDependencyValidationException(Xeption innerException)
            : base("Speech dependency validation error occured fix the errors", innerException)
        { }
    }
}

using System;
using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Speeches.Exceptions
{
    public class SpeechDependencyException : Xeption
    {
        public SpeechDependencyException(Exception innerException)
            : base(message: "Speech dependency error occured, contact support",
                  innerException)
        { }
    }
}

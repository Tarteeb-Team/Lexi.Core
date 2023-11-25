using System;
using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Speeches.Exceptions
{
    public class FailedSpeechServiceException : Xeption
    {
        public FailedSpeechServiceException(Exception innerException)
            : base(message: "failed speech service error occured, contact support",
                  innerException)
        { }
    }
}

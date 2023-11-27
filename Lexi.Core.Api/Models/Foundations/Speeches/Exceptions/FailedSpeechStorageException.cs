using System;
using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Speeches.Exceptions
{
    public class FailedSpeechStorageException : Xeption
    {
        public FailedSpeechStorageException(Exception innerException)
            : base(message: "Failed speech storage error occured, contact support",
                  innerException)
        { }
    }
}

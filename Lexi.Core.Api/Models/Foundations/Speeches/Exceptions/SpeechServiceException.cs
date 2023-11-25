using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Speeches.Exceptions
{
    public class SpeechServiceException : Xeption
    {
        public SpeechServiceException(Xeption innerException)
            : base(message: "Speech service error occured, contact support", innerException)
        { }
    }
}

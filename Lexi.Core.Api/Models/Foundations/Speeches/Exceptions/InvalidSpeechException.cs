using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Speeches.Exceptions
{
    public class InvalidSpeechException : Xeption
    {
        public InvalidSpeechException()
            : base(message: "Speech is invalid.")
        { }
    }
}

//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System.Threading.Tasks;
using Xeptions;
using SpeechModel = Lexi.Core.Api.Models.Foundations.Speeches.Speech;
using Lexi.Core.Api.Models.Foundations.Speeches.Exceptions;


namespace Lexi.Core.Api.Services.Foundations.Speeches
{
    public partial class SpeechService
    {
        private delegate ValueTask<SpeechModel> ReturningSpeechFunction();

        private async ValueTask<SpeechModel> TryCatch(ReturningSpeechFunction returningSpeechFunction)
        {
            try
            {
                return await returningSpeechFunction();
            }
            catch (NullSpeechException nullSpeechException)
            {
                throw CreateAndLogValidationException(nullSpeechException);
            }
        }

        private SpeechValidationException CreateAndLogValidationException(Xeption exception)
        {
            var speechValidationException =
                new SpeechValidationException(exception);

            this.loggingBroker.LogError(speechValidationException);

            return speechValidationException;
        }
    }
}

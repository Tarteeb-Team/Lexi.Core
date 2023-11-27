//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using EFxceptions.Models.Exceptions;
using Lexi.Core.Api.Models.Foundations.Speeches.Exceptions;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using Xeptions;
using SpeechModel = Lexi.Core.Api.Models.Foundations.Speeches.Speech;


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
            catch (NotFoundSpeechException notFoundSpeechException)
            {
                throw CreateAndLogValidationException(notFoundSpeechException);
            }
            catch (InvalidSpeechException invalidSpeechException)
            {
                throw CreateAndLogValidationException(invalidSpeechException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistValidationException =
                    new AlreadyExistValidationException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistValidationException);
            }
            catch (SqlException sqlException)
            {
                var failedSpeechStorageException =
                    new FailedSpeechStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedSpeechStorageException);
            }
            catch (Exception exception)
            {
                var failedSpeechServiceException =
                    new FailedSpeechServiceException(exception);

                throw CreateAndLogServiceException(failedSpeechServiceException);
            }
        }

        private SpeechValidationException CreateAndLogValidationException(Xeption exception)
        {
            var speechValidationException =
                new SpeechValidationException(exception);

            this.loggingBroker.LogError(speechValidationException);

            return speechValidationException;
        }

        private SpeechDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var speechDependencyValidationException =
                new SpeechDependencyValidationException(exception);

            this.loggingBroker.LogError(speechDependencyValidationException);

            return speechDependencyValidationException;
        }

        private SpeechDependencyException CreateAndLogCriticalDependencyException(Xeption excpetion)
        {
            var speechDependencyExcpetion = new SpeechDependencyException(excpetion);
            this.loggingBroker.LogCritical(speechDependencyExcpetion);

            return speechDependencyExcpetion;
        }

        private SpeechServiceException CreateAndLogServiceException(Xeption exception)
        {
            var speechServiceException = new SpeechServiceException(exception);
            this.loggingBroker.LogCritical(speechServiceException);

            return speechServiceException;
        }
    }
}
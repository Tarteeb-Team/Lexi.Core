﻿//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System.Threading.Tasks;
using Xeptions;
using SpeechModel = Lexi.Core.Api.Models.Foundations.Speeches.Speech;
using Lexi.Core.Api.Models.Foundations.Speeches.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;


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
            catch(InvalidSpeechException invalidSpeechException)
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
    }
}

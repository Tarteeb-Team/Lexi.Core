//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using System.Data;
using System.Reflection.Metadata;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Models.Foundations.Speeches;
using Lexi.Core.Api.Models.Foundations.Speeches.Exceptions;
using Lexi.Core.Api.Models.Foundations.Users;
using SpeechModel = Lexi.Core.Api.Models.Foundations.Speeches.Speech;

namespace Lexi.Core.Api.Services.Foundations.Speeches
{
    public partial class SpeechService
    {
        private void ValidateSpeechOnAdd(SpeechModel speech)
        {
            ValidateSpeechNotNull(speech);

            Validate(
                (Rule: IsInvalid(speech.Id), Parameter: nameof(SpeechModel.Id)),
                (Rule: IsInvalid(speech.Sentence), Parameter: nameof(SpeechModel.Sentence)),
                (Rule: IsInvalid(speech.UserId), Parameter: nameof(SpeechModel.UserId)),
                (Rule: IsInvalid(speech.User), Parameter: nameof(SpeechModel.User)),
                (Rule: IsInvalid(speech.Feedbacks), Parameter: nameof(SpeechModel.Feedbacks)));
        }

        private void ValidateStorageSpeechExists(Speech maybeSpeech, Guid id)
        {
            if (maybeSpeech is null)
            {
                throw new NotFoundSpeechException(id);
            }
        }

        private void ValidateSpeechId(Guid id)
        {
            Validate(
                (Rule: IsInvalid(id), Parameter: nameof(SpeechModel.Id)));
        }
        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(string text) => new
        {

            Condition = System.String.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };
        private static dynamic IsInvalid(User user) => new
        {
            Condition = user == null,
            Message = "User is required"
        };
        private static dynamic IsInvalid(Feedback feedback) => new
        {
            Condition = feedback == null,
            Message = "Feedback is required"
        };

        private void ValidateSpeechNotNull(SpeechModel speech)
        {
            if (speech == null)
                throw new NullSpeechException();
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidSpeechException = new InvalidSpeechException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidSpeechException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidSpeechException.ThrowIfContainsErrors();
        }
    }
}
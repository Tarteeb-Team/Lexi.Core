//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System.Data;
using System.Reflection.Metadata;
using Lexi.Core.Api.Models.Foundations.Speeches;
using Lexi.Core.Api.Models.Foundations.Users;
using Lexi.Core.Api.Models.Foundations.Users.Exceptions;
using System;

namespace Lexi.Core.Api.Services.Foundations.Users
{
    public partial class UserService
    {
        private void ValidateUserOnAdd(User user)
        {
            ValidateUserNotNull(user);

            Validate(
                (Rule: IsInvalid(user.Id), Parameter: nameof(User.Id)),
                (Rule: IsInvalid(user.Name), Parameter: nameof(User.Name)));
        }
        private void ValidateUserModify(User user)
        {
            ValidateUserNotNull(user);

            Validate(
                (Rule: IsInvalid(user.Id), Parameter: nameof(User.Id)),
                (Rule: IsInvalid(user.Name), Parameter: nameof(User.Name)));
        }
        private void ValidateAgainstStorageUserOnModify(User user, User storageUser)
        {
            ValidateStorageUser(user, storageUser.Id);

            Validate(
               (Rule: IsInvalid(user.Id), Parameter: nameof(User.Id)),
                (Rule: IsInvalid(user.Name), Parameter: nameof(User.Name)));
        }
        private void ValidateStorageUser(User maybeUser, Guid id)
        {
            if (maybeUser is null)
            {
                throw new NotFoundUserException(id);
            }
        }
        private void ValidateUserId(Guid id)
        {
            Validate(
                (Rule: IsInvalid(id), Parameter: nameof(User.Id)));
        } 
        private static void ValidateUserNotNull(User user)
        {
            if (user == null)
            {
                throw new NullUserException();
            }
        }

        private dynamic IsInvalid(Guid id) => new
        {
            Condition = id == default,
            Message = "Id is required"
        };

        private dynamic IsInvalid(string text) => new
        {
            Condition = string.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidUserException = new InvalidUserException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidUserException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }
            invalidUserException.ThrowIfContainsErrors();
        }
    }
}

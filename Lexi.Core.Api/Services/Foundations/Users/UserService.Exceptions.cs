//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using Lexi.Core.Api.Models.Foundations.Users;
using Lexi.Core.Api.Models.Foundations.Users.Exceptions;
using Xeptions;

namespace Lexi.Core.Api.Services.Foundations.Users
{
    public partial class UserService
    {
        private delegate ValueTask<User> ReturningUserFunction();
        
        private async ValueTask<User> TryCatch(ReturningUserFunction returningUserFunction)
        {
            try
            {
                return await returningUserFunction();
            }
            catch (NullUserException nullUserException)
            {
                throw CreateAndLogValidationException(nullUserException);
            }
            catch(InvalidUserException invalidUserException)
            {
                throw CreateAndLogValidationException(invalidUserException);
            }
            catch(DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsUserException =
                    new UserAlreadyExistsException(duplicateKeyException);

                throw CreateAndALogDependencyValidationException(alreadyExistsUserException);
            }
        }

        private UserDependencyValidationException CreateAndALogDependencyValidationException(Xeption exception)
        {
            var userDependencyValidationException =
                new UserDependencyValidationException(exception);
            this.loggingBroker.LogError(userDependencyValidationException);

            return userDependencyValidationException;
        }

        private UserValidationException CreateAndLogValidationException(Xeption exception)
        {
            var userValidationException = new UserValidationException(exception);
            this.loggingBroker.LogError(userValidationException);

            return userValidationException;
        }
    }
}

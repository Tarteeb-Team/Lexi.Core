//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using EFxceptions.Models.Exceptions;
using Lexi.Core.Api.Models.Foundations.Users;
using Lexi.Core.Api.Models.Foundations.Users.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xeptions;

namespace Lexi.Core.Api.Services.Foundations.Users
{
    public partial class UserService
    {
        private delegate ValueTask<User> ReturningUserFunction();
        private delegate IQueryable<User> ReturningUsersFunction();

        private IQueryable<User> TryCatch(ReturningUsersFunction returningusersFunction)
        {
            try
            {
                return returningusersFunction();
            }
            catch (SqlException sqlExeption) 
            {
                var failedUserStorageException = new FailedUserStorageException(sqlExeption);
                throw CreateAndLogCriticalDependencyException(failedUserStorageException);
            }
            catch (Exception exception) 
            {
                var failedUserServiceException = new FailedUserServiceException(exception);
                throw CreateAndLogServiceException(failedUserServiceException);
            }
        }
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
            catch (InvalidUserException invalidUserException)
            {
                throw CreateAndLogValidationException(invalidUserException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsUserException =
                    new UserAlreadyExistsException(duplicateKeyException);

                throw CreateAndALogDependencyValidationException(alreadyExistsUserException);
            }
            catch (NotFoundUserException notFoundUserException)
            {
                throw CreateAndLogValidationException(notFoundUserException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedUserException =
                   new LockedUserException(dbUpdateConcurrencyException);

                throw CreateAndLogDependencyException(lockedUserException);
            }
            catch (SqlException sqlException)
            {
                var failedUserStorageException = new FailedUserStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedUserStorageException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedUserStorageException =
                     new FailedUserStorageException(dbUpdateException);

                throw CreateAndLogDependencyException(failedUserStorageException);
            }
            catch (Exception exception)
            {
                var failedUserServiceException = new FailedUserServiceException(exception);

                throw CreateAndLogServiceException(failedUserServiceException);
            }
        }
        private UserServiceException CreateAndLogServiceException(Xeption exception)
        {
            var userServiceException = new UserServiceException(exception);
            this.loggingBroker.LogError(userServiceException);

            return userServiceException;
        }
        private UserDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            UserDependencyException userDependencyException =
                new UserDependencyException(exception);

            this.loggingBroker.LogError(userDependencyException);

            return userDependencyException;
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
        private UserDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var userDependencyException = new UserDependencyException(exception);
            this.loggingBroker.LogCritical(userDependencyException);

            return userDependencyException;
        }
    }
}

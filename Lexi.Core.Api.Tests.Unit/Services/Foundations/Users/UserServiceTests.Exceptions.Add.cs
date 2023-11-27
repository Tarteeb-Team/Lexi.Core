//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using EFxceptions.Models.Exceptions;
using FluentAssertions;
using Lexi.Core.Api.Models.Foundations.Users;
using Lexi.Core.Api.Models.Foundations.Users.Exceptions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateKeyErrorOccursAndLogItAsync()
        {
            //given 
            string someMessage = GetRandomString();
            User randomUser = CreateRandomUser();

            var duplicateKeyException =
                new DuplicateKeyException(someMessage);

            var userAlreadyExcistsException =
                new UserAlreadyExistsException(duplicateKeyException);

            var excpectedUserDependencyValidationException =
               new UserDependencyValidationException(userAlreadyExcistsException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertUserAsync(randomUser)).ThrowsAsync(duplicateKeyException);
            //when
            ValueTask<User> addUserTask =
                   this.userService.AddUserAsync(randomUser);

            var actualUserDependencyValidationException =
                await Assert.ThrowsAnyAsync<UserDependencyValidationException>(addUserTask.AsTask);
            //then

            actualUserDependencyValidationException.
                Should().
                BeEquivalentTo(excpectedUserDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
             broker.InsertUserAsync(randomUser), Times.Once);


            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    excpectedUserDependencyValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            //given 
            User randomUser = CreateRandomUser();

            DbUpdateConcurrencyException dbUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            LockedUserException lockedUserException =
                new LockedUserException(dbUpdateConcurrencyException);

            UserDependencyException expectedUserDependencyException =
                 new UserDependencyException(lockedUserException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertUserAsync(randomUser)).ThrowsAsync(dbUpdateConcurrencyException);
            //when
            ValueTask<User> addUserTask =
                   this.userService.AddUserAsync(randomUser);

            var actualUserDependencyException =
                await Assert.ThrowsAnyAsync<UserDependencyException>(addUserTask.AsTask);
            //then

            actualUserDependencyException.
                Should().
                BeEquivalentTo(expectedUserDependencyException);

            this.storageBrokerMock.Verify(broker =>
             broker.InsertUserAsync(randomUser), Times.Once);


            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateErrorOccursAndLogItAsync()
        {
            //given 
            User randomUser = CreateRandomUser();

            DbUpdateException dbUpdateException =
                new DbUpdateException();

            var failedUserStorageException =
                new FailedUserStorageException(dbUpdateException);

            UserDependencyException expectedUserDependencyException =
                 new UserDependencyException(failedUserStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertUserAsync(randomUser)).ThrowsAsync(dbUpdateException);
            //when
            ValueTask<User> addUserTask =
                   this.userService.AddUserAsync(randomUser);

            var actualUserDependencyException =
                await Assert.ThrowsAnyAsync<UserDependencyException>(addUserTask.AsTask);
            //then

            actualUserDependencyException.
                Should().
                BeEquivalentTo(expectedUserDependencyException);

            this.storageBrokerMock.Verify(broker =>
             broker.InsertUserAsync(randomUser), Times.Once);


            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccursAndLogItAsync()
        {
            //given
            User randomUser = CreateRandomUser();

            var exception = new Exception();

            var failedUserServiceException =
                new FailedUserServiceException(exception);

            var expectedUserServiceException =
                new UserServiceException(failedUserServiceException);

            this.storageBrokerMock.Setup(broker =>
               broker.InsertUserAsync(randomUser)).ThrowsAsync(failedUserServiceException);
            //when

            ValueTask<User> addUserTask =
                this.userService.AddUserAsync(randomUser);


            var actualUserServiceException =
                await Assert.ThrowsAnyAsync<UserServiceException>(addUserTask.AsTask);

            //then

            actualUserServiceException.Should().BeEquivalentTo(expectedUserServiceException);

            this.storageBrokerMock.Verify(broker =>
                 broker.InsertUserAsync(randomUser), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs
                (expectedUserServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

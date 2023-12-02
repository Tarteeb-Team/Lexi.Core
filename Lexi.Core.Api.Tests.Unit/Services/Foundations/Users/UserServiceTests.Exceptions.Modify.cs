//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using FluentAssertions;
using Lexi.Core.Api.Models.Foundations.Users;
using Lexi.Core.Api.Models.Foundations.Users.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given 
            User randomUser = CreateRandomUser();
            User someUser = randomUser;
            Guid userId = someUser.Id;
            SqlException sqlException = CreateSqlException();

            var failedUserStorageException =
                new FailedUserStorageException(sqlException);

            var expectedUserDependencyException =
                new UserDependencyException(failedUserStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(userId))
                    .ThrowsAsync(sqlException);

            // when 
            ValueTask<User> modifyUserTask =
                this.userService.ModifyUserAsync(someUser);

            UserDependencyException actualUserDependencyException =
                await Assert.ThrowsAsync<UserDependencyException>(
                    modifyUserTask.AsTask);

            // then
            actualUserDependencyException.Should()
                .BeEquivalentTo(expectedUserDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(userId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogCritical(It.Is(SameExceptionAs(
                   expectedUserDependencyException))), Times.Once);
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given 
            User randomUser = CreateRandomUser();
            User someUser = randomUser;
            Guid userId = someUser.Id;
            var databaseUpdateException = new DbUpdateException();

            var failedUserStorageException =
                new FailedUserStorageException(databaseUpdateException);

            var expectedUserDependencyException =
                new UserDependencyException(failedUserStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(userId))
                    .ThrowsAsync(databaseUpdateException);

            // when 
            ValueTask<User> modifyUserTask =
                this.userService.ModifyUserAsync(someUser);

            UserDependencyException actualUserDependencyException =
                await Assert.ThrowsAsync<UserDependencyException>(modifyUserTask.AsTask);

            // then
            actualUserDependencyException.Should()
                .BeEquivalentTo(expectedUserDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(userId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
        [Fact]
        public async Task
            ShouldThrowDependencyValidationExceptionOnModifyIfDatabaseUpdateConcurrencyExceptionOccursAndLogItAsync()
        {
            // given 
            User randomUser = CreateRandomUser();
            User someUser = randomUser;
            Guid userId = someUser.Id;
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedUserException =
                new LockedUserException(databaseUpdateConcurrencyException);

            var expectedUserDependencyValidationException =
                 new UserDependencyValidationException(lockedUserException);

            this.storageBrokerMock.Setup(broker =>
                    broker.SelectUserByIdAsync(userId)).ThrowsAsync(databaseUpdateConcurrencyException);

            // when 
            ValueTask<User> modifyUserTask =
                this.userService.ModifyUserAsync(someUser);

            UserDependencyValidationException actualUserDependencyException =
                await Assert.ThrowsAsync<UserDependencyValidationException>(modifyUserTask.AsTask);

            // then
            actualUserDependencyException.Should()
                .BeEquivalentTo(expectedUserDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(userId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserDependencyValidationException))));

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfExceptionOccursAndLogItAsync()
        {
            // given 
            User randomUser = CreateRandomUser();
            User someUser = randomUser;
            Guid userId = someUser.Id;
            var serviceException = new Exception();

            var failedUserServiceException =
                new FailedUserServiceException(serviceException);

            var expectedUserServiceException =
                new UserServiceException(failedUserServiceException);

            this.storageBrokerMock.Setup(broker =>
                    broker.SelectUserByIdAsync(userId)).ThrowsAsync(serviceException);

            // when 
            ValueTask<User> modifyUserTask =
                this.userService.ModifyUserAsync(someUser);

            UserServiceException actualUserServiceException =
                await Assert.ThrowsAsync<UserServiceException>(modifyUserTask.AsTask);

            // then
            actualUserServiceException.Should()
                .BeEquivalentTo(expectedUserServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(userId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedUserServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

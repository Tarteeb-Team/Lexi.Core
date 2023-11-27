//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Lexi.Core.Api.Models.Foundations.Users;
using Lexi.Core.Api.Models.Foundations.Users.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async Task ShouldThrowSqlExceptionOnRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedUserStorageException = new FailedUserStorageException(sqlException);

            var expectedUserDependencyException =
                new UserDependencyException(failedUserStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(someId)).ThrowsAsync(sqlException);

            // when
            ValueTask<User> removeUserTask = this.userService.RemoveUserAsync(someId);

            UserDependencyException actualUserDependencyException =
                await Assert.ThrowsAsync<UserDependencyException>(removeUserTask.AsTask);

            // then
            actualUserDependencyException.Should()
                .BeEquivalentTo(expectedUserDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(expectedUserDependencyException))),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

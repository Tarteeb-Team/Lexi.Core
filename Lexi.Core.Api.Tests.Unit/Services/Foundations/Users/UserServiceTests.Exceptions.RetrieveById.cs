using FluentAssertions;
using Lexi.Core.Api.Models.Foundations.Users;
using Lexi.Core.Api.Models.Foundations.Users.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdAsyncIfSqlErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedUserStorageException =
                    new FailedUserStorageException(sqlException);

            UserDependencyException expectedUserDependencyException =
                    new UserDependencyException(failedUserStorageException);

            this.storageBrokerMock.Setup(broker =>
            broker.SelectUserByIdAsync(It.IsAny<Guid>())).
                ThrowsAsync(sqlException);
            //when
            ValueTask<User> retrieveUserByIdTask =
                    this.userService.RetrieveUserByIdAsync(someId);

            UserDependencyException actualUserDependencyException =
                await Assert.ThrowsAsync<UserDependencyException>
                                (retrieveUserByIdTask.AsTask);
            //then
            actualUserDependencyException.
                Should().
                BeEquivalentTo(expectedUserDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(
                expectedUserDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdAsyncIfServiceErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            Exception serviceException = new Exception();

            var failedUserServiceException =
                    new FailedUserServiceException(serviceException);

            var expectedUserServiceException = new
                    UserServiceException(failedUserServiceException);

            this.storageBrokerMock.Setup(broker =>
                 broker.SelectUserByIdAsync(It.IsAny<Guid>())).ThrowsAsync(serviceException);
            //when
            ValueTask<User> retrieveUserByIdTask =
                 this.userService.RetrieveUserByIdAsync(someId);

            UserServiceException actualUserServiceException =
                await Assert.ThrowsAsync<UserServiceException>(
                    retrieveUserByIdTask.AsTask);
            //then
            actualUserServiceException.
                Should().
                BeEquivalentTo(expectedUserServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                expectedUserServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}


using FluentAssertions;
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
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSqlErrorOccuredAndLogIt()
        {
            // given
            SqlException sqlException = CreateSqlException();

            var failedUserStorageException =
                new FailedUserStorageException(sqlException);

            var expectedUserDependencyException =
                new UserDependencyException(failedUserStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllUsers())
                    .Throws(sqlException);

            // when
            Action retrieveAllUsersAction = () =>
               this.userService.RetrieveAllUsers();

            UserDependencyException actualUserDependencyException =
                Assert.Throws<UserDependencyException>(
                    retrieveAllUsersAction);

            // then
            actualUserDependencyException.Should().BeEquivalentTo(
               expectedUserDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllUsers(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedUserDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
        [Fact]
        private void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string exceptionMessage = GetRandomString();
            var serviceException = new Exception(exceptionMessage);

            var failedUserServiceException =
                new FailedUserServiceException(serviceException);

            var expectedUserServiceException =
                new UserServiceException(failedUserServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllUsers())
                    .Throws(serviceException);

            // when
            Action retrieveAllUsersActions = () =>
                this.userService.RetrieveAllUsers();

            UserServiceException actualUserServiceException =
                Assert.Throws<UserServiceException>(
                    retrieveAllUsersActions);

            // then
            actualUserServiceException.Should().BeEquivalentTo(expectedUserServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllUsers(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using FluentAssertions;
using Lexi.Core.Api.Models.Foundations.Users;
using Lexi.Core.Api.Models.Foundations.Users.Exceptions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            Guid invalidUserId = Guid.Empty;
            var invalidUserException = new InvalidUserException();

            invalidUserException.AddData(
                key: nameof(User.Id),
                values: "Id is required");

            var expectedUserValidationException =
                new UserValidationException(invalidUserException);

            //when
            ValueTask<User> retrieveUserByIdTask =
                this.userService.RetrieveUserByIdAsync(invalidUserId);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(
                    retrieveUserByIdTask.AsTask);

            //then
            actualUserValidationException.Should().BeEquivalentTo(expectedUserValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(
                    SameExceptionAs(expectedUserValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
               broker.SelectUserByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfUserNotFoundAndLogItAsync()
        {
            //given
            Guid someUserId = Guid.NewGuid();
            User noUser = null;

            var notFoundUserException =
                new NotFoundUserException(someUserId);

            var expectedValidationException =
                new UserValidationException(notFoundUserException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noUser);

            //when
            ValueTask<User> retrieveUserByIdTask =
                this.userService.RetrieveUserByIdAsync(someUserId);

            UserValidationException actualValidationException =
                await Assert.ThrowsAsync<UserValidationException>(
                    retrieveUserByIdTask.AsTask);

            //then 
            actualValidationException.Should().BeEquivalentTo(expectedValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

    }
}

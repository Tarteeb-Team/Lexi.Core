﻿//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using FluentAssertions;
using Lexi.Core.Api.Models.Foundations.Users;
using Lexi.Core.Api.Models.Foundations.Users.Exceptions;
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
        public async Task ShouldThrowValidationExceptionOnModifyIfUserIsNullAndLogItAsync()
        {
            // given
            User nullUser = null;
            var nullUserException = new NullUserException();

            var expectedUserValidationException =
                new UserValidationException(nullUserException);

            // when
            ValueTask<User> modifyUserTask =
                this.userService.ModifyUserAsync(nullUser);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(
                    modifyUserTask.AsTask);

            // then
            actualUserValidationException.Should()
                .BeEquivalentTo(expectedUserValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserAsync(It.IsAny<User>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfUserIsInvalidAndLogItAsync(string invalidString)
        {
            //given
            User invalidUser = new User
            {
                Name = invalidString
            };

            var invalidUserException =
                new InvalidUserException();

            invalidUserException.AddData(
                key: nameof(User.Id),
                values: "Id is required");

            invalidUserException.AddData(
                key: nameof(User.Name),
                values: "Text is required");

            var expectedUserValidationException =
                new UserValidationException(invalidUserException);

            // when
            ValueTask<User> modifyUserTask =
                this.userService.ModifyUserAsync(invalidUser);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(
                    modifyUserTask.AsTask);

            // then
            actualUserValidationException.Should()
                .BeEquivalentTo(expectedUserValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserAsync(It.IsAny<User>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUserDoesNotExistAndLogItAsync()
        {
            // given
            User randomUser = CreateRandomUser();
            User nonExistUser = randomUser;
            User nullUser = null;

            var notFoundUserException =
                new NotFoundUserException(nonExistUser.Id);

            var expectedUserValidationException =
                new UserValidationException(notFoundUserException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(nonExistUser.Id))
                    .ReturnsAsync(nullUser);

            // when
            ValueTask<User> modifyUserTask =
                this.userService.ModifyUserAsync(nonExistUser);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>
                    (modifyUserTask.AsTask);

            // then
            actualUserValidationException.Should().BeEquivalentTo(expectedUserValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(nonExistUser.Id), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserAsync(nonExistUser), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

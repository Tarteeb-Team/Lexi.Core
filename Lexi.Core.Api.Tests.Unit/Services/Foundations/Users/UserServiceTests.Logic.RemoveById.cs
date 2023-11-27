//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Lexi.Core.Api.Models.Foundations.Users;
using Moq;
using Xunit;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async Task ShouldRemoveUserByIdAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputId = randomId;
            User randomUser = CreateRandomUser();
            User persistedUser = randomUser;
            User expectedInputUser = persistedUser;
            User deletedUser = expectedInputUser;
            User expectedUser = deletedUser.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputId))
                    .ReturnsAsync(persistedUser);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteUserAsync(expectedInputUser))
                    .ReturnsAsync(deletedUser);

            // when
            User actualUser =
                await this.userService
                    .RemoveUserAsync(inputId);

            // then
            actualUser.Should().BeEquivalentTo(expectedUser);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteUserAsync(expectedInputUser),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

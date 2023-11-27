//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using FluentAssertions;
using Force.DeepCloner;
using Lexi.Core.Api.Models.Foundations.Users;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveUserByIdAsync()
        {
            //given
            Guid randomUserId = Guid.NewGuid();
            Guid inputUserId = randomUserId;
            User randomUser = CreateRandomUser();
            User persistedUser = randomUser;
            User expectedUser = persistedUser.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                .ReturnsAsync(expectedUser);

            //when
            User actualUser =
                  await this.userService
                      .RetrieveUserByIdAsync(inputUserId);

            //then
            actualUser.Should().BeEquivalentTo(expectedUser);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputUserId),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

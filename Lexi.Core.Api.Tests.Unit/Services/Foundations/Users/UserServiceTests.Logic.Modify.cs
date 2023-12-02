﻿//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using FluentAssertions;
using Force.DeepCloner;
using Lexi.Core.Api.Models.Foundations.Users;
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
        public async Task ShouldModifyUserAsync()
        {
            // given 
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            User persistedUser = inputUser.DeepClone();
            User updatedUser = inputUser;
            User expectedUser = updatedUser.DeepClone();
            Guid InputUserId = inputUser.Id;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(InputUserId))
                    .ReturnsAsync(persistedUser);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateUserAsync(inputUser))
                    .ReturnsAsync(updatedUser);

            //when
            User actualUser =
                await this.userService.ModifyUserAsync(inputUser);

            // then
            actualUser.Should().BeEquivalentTo(expectedUser);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(InputUserId), Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserAsync(inputUser), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Brokers.Loggings;
using Lexi.Core.Api.Brokers.Storages;
using Lexi.Core.Api.Models.Foundations.Users;
using Lexi.Core.Api.Services.Foundations.Users;
using Moq;
using System;
using System.Linq.Expressions;
using Tynamix.ObjectFiller;
using Xeptions;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.NewFolder.Users
{
    public partial class UserServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IUserService userService;

        public UserServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.userService = new UserService(
                storageBroker: this.storageBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }
        private string GetRandomString() =>
           new MnemonicString().GetValue();
        private static User CreateRandomUser() =>
            CreateAccountFiller().Create();

        private static Filler<User> CreateAccountFiller()
        {
            var filler = new Filler<User>();

            return filler;
        }

        private Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);
    }
}

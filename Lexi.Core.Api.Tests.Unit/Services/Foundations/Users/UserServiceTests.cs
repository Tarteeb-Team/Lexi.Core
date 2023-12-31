//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System.Runtime.Serialization;
using Lexi.Core.Api.Brokers.Loggings;
using Lexi.Core.Api.Brokers.Storages;
using Lexi.Core.Api.Models.Foundations.Users;
using Lexi.Core.Api.Services.Foundations.Users;
using Microsoft.Data.SqlClient;
using Moq;
using System;
using System.Linq.Expressions;
using Tynamix.ObjectFiller;
using Xeptions;
using System.Linq;
using Lexi.Core.Api.Models.Foundations.Feedbacks;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Users
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
        private IQueryable<User> CreateRandomUsers() =>
            CreateAccountFiller().Create(5).AsQueryable();
        private SqlException CreateSqlException() =>
           (SqlException)FormatterServices.
            GetUninitializedObject(typeof(SqlException));
        private static Filler<User> CreateAccountFiller()
        {
            var filler = new Filler<User>();

            return filler;
        }

        private Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);
    }
}

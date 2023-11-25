//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Brokers.Loggings;
using Lexi.Core.Api.Brokers.Storages;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Models.Foundations.Feedbacks.Exceptions;
using Lexi.Core.Api.Services.Foundations.Feedbacks;
using Moq;
using System.Linq.Expressions;
using System;
using Tynamix.ObjectFiller;
using Xeptions;
using Microsoft.Data.SqlClient;
using System.Runtime.Serialization;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Feedbacks
{
    public partial class FeedbackServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IFeedbackService feedbackService;

        public FeedbackServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.feedbackService = 
                new FeedbackService(
                    storageBroker: this.storageBrokerMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        private Feedback CreateRandomFeedback() =>
            CreateFeedbackFiller().Create();

        private static Filler<Feedback> CreateFeedbackFiller()
        {
            Filler<Feedback> filler = new Filler<Feedback>();

            return filler;
        }

        private Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private SqlException GetSqlError() =>
            (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));

        private string GetRandomString() =>
            new MnemonicString().GetValue();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Lexi.Core.Api.Models.Foundations.Speeches.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;
using SpeechModel = Lexi.Core.Api.Models.Foundations.Speeches.Speech;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Speech
{
    public partial class SpeechServiceTests
    {
        [Fact]
        public async Task ShoulThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {   //given
            SqlException sqlException = GetSqlError();

            var failedSpeechServiceException =
                new FailedSpeechServiceException(sqlException);

            var expectedSpeechDependencyException =
                new SpeechDependencyException(failedSpeechServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllSpeeches()).Throws(sqlException);
            //when

            Action retrieveAllSpeechAction = () =>
                    this.speechService.RetrieveAllSpeeches();

            SpeechDependencyException actualSpeechDependencyException =
                Assert.Throws<SpeechDependencyException>(retrieveAllSpeechAction);

            //then
            actualSpeechDependencyException.
                Should().
                BeEquivalentTo(expectedSpeechDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllSpeeches(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(
                expectedSpeechDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

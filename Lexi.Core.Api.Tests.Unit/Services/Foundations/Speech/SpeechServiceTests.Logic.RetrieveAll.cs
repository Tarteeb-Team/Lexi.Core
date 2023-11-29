//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using SpeechModel = Lexi.Core.Api.Models.Foundations.Speeches.Speech;

namespace Lexi.Core.Api.Tests.Unit.Services.Foundations.Speech
{
    public partial class SpeechServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllSpeeches()
        {
            //given
            IQueryable<SpeechModel> randomSpeeches = CreateRandomSpeeches();
            IQueryable<SpeechModel> storageSpeeches = randomSpeeches;
            IQueryable<SpeechModel> expextedSpeeches = storageSpeeches;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllSpeeches()).Returns(storageSpeeches);
            //when

            IQueryable<SpeechModel> actualSpeeches =
                    this.speechService.RetrieveAllSpeeches();
            //then
            actualSpeeches.Should().BeEquivalentTo(expextedSpeeches);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllSpeeches(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Brokers.TelegramBroker;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Models.Foundations.Speeches;
using Lexi.Core.Api.Models.Foundations.Users;
using Lexi.Core.Api.Models.ObjcetModels;
using Lexi.Core.Api.Services.Cognitives;
using Lexi.Core.Api.Services.Orchestrations.Cognitive;
using Lexi.Core.Api.Services.Orchestrations.Speech;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using SpeechModel = Lexi.Core.Api.Models.Foundations.Speeches.Speech;

namespace Lexi.Core.Api.Services.Orchestrations
{
    public class OrchestrationService : IOrchestrationService
    {
        private readonly ICognitiveOrchestrationService cognitiveOrchestrationService;
        private readonly ISpeechOrchestrationService speechOrchestrationService;

        public OrchestrationService(ICognitiveOrchestrationService cognitiveOrchestrationService,
            ISpeechOrchestrationService speechOrchestrationService)
        {
            this.cognitiveOrchestrationService = cognitiveOrchestrationService;
            this.speechOrchestrationService = speechOrchestrationService;
        }

        public async ValueTask GenerateSpeechFeedbackForUser()
        {
            //this.cognitiveOrchestrationService.StartListening();

            ResponseCognitive responseCognitive = 
                await this.cognitiveOrchestrationService.GetResponseCognitive();


            User user = await this.cognitiveOrchestrationService.AddNewUserAsync();

            SpeechModel speech = await speechOrchestrationService.MapToSpeech(responseCognitive, user.Id);

            Feedback feedback =
                await this.speechOrchestrationService.MapToFeedback(responseCognitive, speech.Id);

            await this.cognitiveOrchestrationService
                .MapFeedbackToStringAndSendMessage(user.TelegramId, feedback);
        }

        public ValueTask<Feedback> RemoveFeedbackAsync(Feedback feedback) =>
            this.speechOrchestrationService.RemoveFeedbackAsync(feedback);

        public ValueTask<Models.Foundations.Speeches.Speech> RemoveSpeechAsync(
            Models.Foundations.Speeches.Speech speechModel) =>
            this.speechOrchestrationService.RemoveSpeechAsync(speechModel);

        public IQueryable<Feedback> RetrieveAllFeedbacks() =>
            this.speechOrchestrationService?.RetrieveAllFeedbacks();

        public IQueryable<Models.Foundations.Speeches.Speech> RetrieveAllSpeeches() =>
            this.speechOrchestrationService.RetrieveAllSpeeches();

        public ValueTask<Feedback> RetrieveFeedbackByIdAsync(Guid id) =>
            this.speechOrchestrationService.RetrieveFeedbackByIdAsync(id);

        public ValueTask<Models.Foundations.Speeches.Speech> RetrieveSpeechByIdAsync(Guid id) =>
            this.speechOrchestrationService.RetrieveSpeechByIdAsync(id);
    }
}

//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Models.ObjcetModels;
using Lexi.Core.Api.Services.Orchestrations.Cognitive;
using Lexi.Core.Api.Services.Orchestrations.Speech;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<ResponseCognitive> GetOggFile(Stream stream)
        {
            ResponseCognitive responseCognitive = await this.cognitiveOrchestrationService.GetOggFile(stream);

            await speechOrchestrationService.MapToSpeech(responseCognitive);
            await speechOrchestrationService.MapToFeedback(responseCognitive);

            return responseCognitive;
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

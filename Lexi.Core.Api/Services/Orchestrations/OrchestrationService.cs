//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Models.ObjcetModels;
using Lexi.Core.Api.Services.Cognitives;
using Lexi.Core.Api.Services.Orchestrations.Cognitive;
using Lexi.Core.Api.Services.Orchestrations.Speech;
using Microsoft.AspNetCore.Http;

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

        public IQueryable<Feedback> RetrieveAllFeedbacks() =>
            this.speechOrchestrationService?.RetrieveAllFeedbacks();

        public ValueTask<Feedback> RetrieveFeedbackByIdAsync(Guid id) =>
            this.speechOrchestrationService.RetrieveFeedbackByIdAsync(id);
    }
}

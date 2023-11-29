//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Models.ObjcetModels;
using Lexi.Core.Api.Services.Foundations.Feedbacks;
using Lexi.Core.Api.Services.Foundations.Speeches;
using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using SpeechModel = Lexi.Core.Api.Models.Foundations.Speeches.Speech;

namespace Lexi.Core.Api.Services.Orchestrations.Speech
{
    public class SpeechOrchestrationService : ISpeechOrchestrationService
    {
        private readonly ISpeechService speechService;
        private readonly IFeedbackService feedbackService;
        private Guid _speechId;

        public SpeechOrchestrationService(IFeedbackService feedbackService,
            ISpeechService speechService)
        {
            this.feedbackService = feedbackService;
            this.speechService = speechService;
        }

        public ValueTask<Feedback> RemoveFeedbackAsync(Feedback feedback) =>
            this.RemoveFeedbackAsync(feedback);

        public IQueryable<Feedback> RetrieveAllFeedbacks() =>
            this.feedbackService.RetrieveAllFeedbacks();

        public ValueTask<Feedback> RetrieveFeedbackByIdAsync(Guid id) =>
            this.feedbackService.RetrieveFeedbackByIdAsync(id);

        public async ValueTask<Feedback> MapToFeedback(ResponseCognitive responseCognitive, Guid speechId)
        {
            Feedback feedback = new Feedback()
            {
                Id = Guid.NewGuid(),
                Accuracy = responseCognitive.NBest[0].PronunciationAssessment.AccuracyScore,
                Complenteness = responseCognitive.NBest[0].PronunciationAssessment.CompletenessScore,
                Fluency = responseCognitive.NBest[0].PronunciationAssessment.FluencyScore,
                Prosody = responseCognitive.NBest[0].PronunciationAssessment.ProsodyScore,
                Pronunciation = responseCognitive.NBest[0].PronunciationAssessment.PronScore,
                SpeechId = speechId
            };

            return await this.feedbackService.AddFeedbackAsync(feedback);
        }

        public async ValueTask<SpeechModel> MapToSpeech(ResponseCognitive responseCognitive, Guid userId)
        {
            SpeechModel speech = new SpeechModel()
            {
                Id = Guid.NewGuid(),
                Sentence = responseCognitive.DisplayText,
                UserId = userId
            };

            _speechId = speech.Id;
            return await speechService.AddSpechesAsync(speech);
        }

        public IQueryable<SpeechModel> RetrieveAllSpeeches() =>
            this.speechService.RetrieveAllSpeeches();

        public ValueTask<SpeechModel> RetrieveSpeechByIdAsync(Guid id) =>
            this.speechService.RetrieveSpeechesByIdAsync(id);

        public ValueTask<SpeechModel> RemoveSpeechAsync(SpeechModel speechModel) =>
            this.speechService.RemoveSpeechAsync(speechModel);
    }
}

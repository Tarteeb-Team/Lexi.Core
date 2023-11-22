//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using System.Threading.Tasks;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using SpeechModel = Lexi.Core.Api.Models.Foundations.Speeches.Speech;
using Lexi.Core.Api.Models.ObjcetModels;
using Lexi.Core.Api.Services.Foundations.Feedbacks;
using Lexi.Core.Api.Services.Foundations.Speeches;
using Lexi.Core.Api.Services.Foundations.Users;

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

        public async Task MapToFeedback(ResponseCognitive responseCognitive)
        {
            Feedback feedback = new Feedback()
            {
                Id = Guid.NewGuid(),
                Accuracy = responseCognitive.NBest[0].PronunciationAssessment.AccuracyScore,
                Complenteness = responseCognitive.NBest[0].PronunciationAssessment.CompletenessScore,
                Fluency = responseCognitive.NBest[0].PronunciationAssessment.FluencyScore,
                Prosody = responseCognitive.NBest[0].PronunciationAssessment.ProsodyScore,
                Pronunciation = responseCognitive.NBest[0].PronunciationAssessment.PronScore,
                SpeechId = _speechId
            };

            await this.feedbackService.AddFeedbackAsync(feedback);
        }

        public async Task MapToSpeech(ResponseCognitive responseCognitive)
        {
            SpeechModel speech = new SpeechModel()
            {
                Id = Guid.NewGuid(),
                Sentence = responseCognitive.DisplayText,
                UserId = new Guid("FF333594-0F6A-4EC4-AAAC-008A57AED5D3")
            };

            _speechId = speech.Id;
            await speechService.AddSpechesAsync(speech);
        }
    }
}

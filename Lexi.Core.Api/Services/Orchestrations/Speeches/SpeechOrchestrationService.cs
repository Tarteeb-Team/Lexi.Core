//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Brokers.UpdateStorages;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Models.ObjcetModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using SpeechModel = Lexi.Core.Api.Models.Foundations.Speeches.Speech;

namespace Lexi.Core.Api.Services.Orchestrations.Speech
{
    public class SpeechOrchestrationService : ISpeechOrchestrationService
    {
        private readonly IUpdateStorageBroker updateStorageBroker;

        private Guid _speechId;

        public SpeechOrchestrationService(
            IUpdateStorageBroker updateStorageBroker)
        {
            this.updateStorageBroker = updateStorageBroker;
        }

        public ValueTask<Feedback> RemoveFeedbackAsync(Feedback feedback) =>
            this.RemoveFeedbackAsync(feedback);

        public IQueryable<Feedback> RetrieveAllFeedbacks() =>
            this.updateStorageBroker.SelectAllFeedbacks();

        public ValueTask<Feedback> RetrieveFeedbackByIdAsync(Guid id) =>
            this.updateStorageBroker.SelectFeedbackByIdAsync(id);

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

            return await this.updateStorageBroker.InsertFeedbackAsync(feedback);
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
            return await updateStorageBroker.InsertSpeechAsync(speech);
        }

        public IQueryable<SpeechModel> RetrieveAllSpeeches() =>
            this.updateStorageBroker.SelectAllSpeeches();

        public ValueTask<SpeechModel> RetrieveSpeechByIdAsync(Guid id) =>
            this.updateStorageBroker.SelectSpeechByIdAsync(id);

        public ValueTask<SpeechModel> RemoveSpeechAsync(SpeechModel speechModel) =>
            this.updateStorageBroker.DeleteSpeechAsync(speechModel);
    }
}

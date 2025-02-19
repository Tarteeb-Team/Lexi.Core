﻿//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using aisha_ai.Services.Foundations.HandleSpeeches;
using Lexi.Core.Api.Brokers.UpdateStorages;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Models.ObjcetModels;
using Lexi.Core.Api.Services.Foundations.ImproveSpeech;
using Lexi.Core.Api.Services.Orchestrations.Cognitive;
using Lexi.Core.Api.Services.Orchestrations.Speech;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SpeechModel = Lexi.Core.Api.Models.Foundations.Speeches.Speech;

namespace Lexi.Core.Api.Services.Orchestrations
{
    public class OrchestrationService : IOrchestrationService
    {
        private readonly ICognitiveOrchestrationService cognitiveOrchestrationService;
        private readonly ISpeechOrchestrationService speechOrchestrationService;
        private readonly IHandleSpeechService handleSpeechService;
        private readonly IOpenAIService openAIService;
        private readonly IUpdateStorageBroker updateStorageBroker;

        public OrchestrationService(ICognitiveOrchestrationService cognitiveOrchestrationService,
            ISpeechOrchestrationService speechOrchestrationService,
            IHandleSpeechService handleSpeechService,
            IOpenAIService openAIService,
            IUpdateStorageBroker updateStorageBroker)
        {
            this.cognitiveOrchestrationService = cognitiveOrchestrationService;
            this.speechOrchestrationService = speechOrchestrationService;
            this.handleSpeechService = handleSpeechService;
            this.openAIService = openAIService;
            this.updateStorageBroker = updateStorageBroker;
        }

        public async ValueTask GenerateSpeechFeedbackForUser(long? telegramId)
        {
            var user = this.updateStorageBroker.SelectAllUsers()
                .FirstOrDefault(u => u.TelegramId == telegramId);

            if (user is null)
            {
                await this.cognitiveOrchestrationService.AddNewUserAsync();

                return;
            }

            ResponseCognitive responseCognitive =
                await this.cognitiveOrchestrationService.GetResponseCognitive();

            SpeechModel speech = await speechOrchestrationService.MapToSpeech(responseCognitive, user.Id);

            string promt = $"If there any grammar mistakes correct and return. If there are not just return my text." +
                $"You should not add any extra text, except if there are some grammar mistakes.";

            var improvedSpeech = await this.openAIService.AnalizeRequestAsync(speech.Sentence, promt);

            user.ImprovedSpeech = improvedSpeech;
            await this.updateStorageBroker.UpdateUserAsync(user);

            await this.handleSpeechService.CreateAndSaveSpeechAudioAsync(improvedSpeech, $"{user.TelegramId}");

            Feedback feedback =
                await this.speechOrchestrationService.MapToFeedback(responseCognitive, speech.Id);

            await this.cognitiveOrchestrationService
                .MapFeedbackToStringAndSendMessage(user.TelegramId, feedback, speech.Sentence);

            string wwwRootPath = Environment.CurrentDirectory;
            string audioFilePath = Path.Combine(wwwRootPath, "wwwroot", "outputWavs", $"{user.TelegramId}.wav");

            if (File.Exists(audioFilePath))
            {
                File.Delete(audioFilePath);
            }
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

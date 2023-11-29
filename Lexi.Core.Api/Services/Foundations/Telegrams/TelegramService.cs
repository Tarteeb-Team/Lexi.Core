//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Brokers.TelegramBroker;
using Lexi.Core.Api.Models.Foundations.ExternalUsers;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Services.Foundations.Telegrams
{
    public class TelegramService : ITelegramService
    {
        private readonly ITelegramBroker telegramBroker;

        public TelegramService(ITelegramBroker telegramBroker)
        {
            this.telegramBroker = telegramBroker;
        }

        public async ValueTask<ExternalUser> GetExternalUserAsync() =>
            await this.telegramBroker.CreateExternalUserAsync();

        public async ValueTask MapFeedbackToStringAndSendMessage(long telegramId, Feedback feedback)
        {
            string readyFeedback = $"Result:\n 1. Accuracy: {feedback.Accuracy}\n" +
                $"Fluency: {feedback.Fluency}\n" +
                $"Prosody: {feedback.Prosody}\n" +
                $"Complenteness: {feedback.Complenteness}\n" +
                $"Pronunciation: {feedback.Pronunciation}";

            await this.telegramBroker.SendTextMessageAsync(telegramId, readyFeedback);
        }

        public void StartListening() =>
            this.telegramBroker.StartListening();
    }
}

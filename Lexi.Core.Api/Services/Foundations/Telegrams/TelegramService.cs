//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Brokers.TelegramBroker;
using Lexi.Core.Api.Models.Foundations.ExternalUsers;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using System.Threading.Tasks;
using System.Web;

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

        public async ValueTask MapFeedbackToStringAndSendMessage(
            long telegramId, Feedback feedback, string sentence)
        {
            string readyFeedback = $"🎓 LexiEnglishBot 🎓\n\n" +
        $"📝 Your sentence: {sentence}\n\n" +
        $"✅ Result:\n\n" +
        $"🤩 Accuracy: {feedback.Accuracy}\n" +
        $"🤓 Fluency: {feedback.Fluency}\n" +
        $"😎 Prosody: {feedback.Prosody}\n" +
        $"🥸 Complateness: {feedback.Complenteness}\n" +
        $"🥳 Pronunciation: {feedback.Pronunciation} \n\n Keep going 💪";

            await this.telegramBroker.SendTextMessageAsync(telegramId, readyFeedback);
        }

        public void StartListening() =>
            this.telegramBroker.StartListening();
    }
}

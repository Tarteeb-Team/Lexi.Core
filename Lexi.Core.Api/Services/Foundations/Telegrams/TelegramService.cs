//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Brokers.TelegramBroker;
using Lexi.Core.Api.Brokers.UpdateStorages;
using Lexi.Core.Api.Models.Foundations.ExternalUsers;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using System.Linq;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Services.Foundations.Telegrams
{
    public class TelegramService : ITelegramService
    {
        private readonly ITelegramBroker telegramBroker;
        private readonly IUpdateStorageBroker updateStorageBroker;

        public TelegramService(ITelegramBroker telegramBroker, IUpdateStorageBroker updateStorageBroker)
        {
            this.telegramBroker = telegramBroker;
            this.updateStorageBroker = updateStorageBroker;
        }

        public async ValueTask<ExternalUser> GetExternalUserAsync() =>
            await this.telegramBroker.CreateExternalUserAsync();

        public async ValueTask MapFeedbackToStringAndSendMessage(
            long telegramId, Feedback feedback, string sentence)
        {
            decimal overall = (feedback.Accuracy + feedback.Prosody + feedback.Fluency +
                feedback.Complenteness + feedback.Pronunciation) / 5;

            var user = this.updateStorageBroker
                .SelectAllUsers().FirstOrDefault(u => u.TelegramId == telegramId);

            if (user.Overall is null)
            {
                user.Overall = overall;
                await this.updateStorageBroker.UpdateUserAsync(user);
            }
            else
            {
                user.Overall += overall;
                user.Overall /= 2;

                await this.updateStorageBroker.UpdateUserAsync(user);
            }

            string readyFeedback = $"🎓 LexiEnglishBot 🎓\n\n" +
        $"📝 Your sentence: {sentence}\n\n" +
        $"✅ Result:\n\n" +
        $"🤩 Accuracy: {feedback.Accuracy}%\n" +
        $"🤓 Fluency: {feedback.Fluency}%\n" +
        $"😎 Prosody: {feedback.Prosody}%\n" +
        $"🥸 Completeness: {feedback.Complenteness}%\n" +
        $"🥳 Pronunciation: {feedback.Pronunciation}%\n\n🔥 Overall: {overall}%\n\nKeep studying 💪🏼";

            await this.telegramBroker.SendTextMessageAsync(telegramId, readyFeedback);
        }

        public void StartListening() =>
            this.telegramBroker.StartListening();
    }
}

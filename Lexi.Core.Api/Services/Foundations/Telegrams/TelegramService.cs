//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Brokers.TelegramBroker;
using Lexi.Core.Api.Models.Foundations.ExternalUsers;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Services.Foundations.Users;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Lexi.Core.Api.Services.Foundations.Telegrams
{
    public class TelegramService : ITelegramService
    {
        private readonly ITelegramBroker telegramBroker;
        private readonly IUserService userService;

        public TelegramService(ITelegramBroker telegramBroker, IUserService userService)
        {
            this.telegramBroker = telegramBroker;
            this.userService = userService;
        }

        public async ValueTask<ExternalUser> GetExternalUserAsync() =>
            await this.telegramBroker.CreateExternalUserAsync();

        public async ValueTask MapFeedbackToStringAndSendMessage(
            long telegramId, Feedback feedback, string sentence)
        {
            decimal overall = (feedback.Accuracy + feedback.Prosody + feedback.Fluency +
                feedback.Complenteness + feedback.Pronunciation) / 5;

            var user = this.userService
                .RetrieveAllUsers().FirstOrDefault(u => u.TelegramId == telegramId);

            if(user.Overall is null)
            {
                user.Overall = overall;
                await this.userService.ModifyUserAsync(user);
            }
            else
            {
                user.Overall += overall;
                user.Overall /= 2;

                await this.userService.ModifyUserAsync(user);
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

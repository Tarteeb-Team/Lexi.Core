using Lexi.Core.Api.Models.Foundations.Users;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Lexi.Core.Api.Brokers.TelegramBroker
{
    public partial class TelegramBroker
    {
        private async ValueTask<bool> TestSpeechPronun(
        ITelegramBotClient client,
        Update update,
        Models.Foundations.Users.User user)
        {
            if (user.State is State.Active && update.Message.Text is "Test pronunciation 🎙")
            {
                await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    replyMarkup: PronunciationMarkup(),
                    text: $"🎓 LexiEnglishBot 🎓\r\n\r\n" +
                          $"You can:\r\n" +
                          $"1. Send a voice message 🎙 to check pronunciation and fluency.\r\n" +
                          $"2. Click 'Generate Question' if it's difficult to think of what to say.\r\n" +
                          $"\r\nI will evaluate your pronunciation and fluency based on your response. 😁");

                user.State = State.TestSpeechPronun;
                await this.updateStorageBroker.UpdateUserAsync(user);

                return true;
            }
            if (user.State is State.Active && update.Message.Text is "Generate a question 🎁")
            {

                
                return true;
            }

            return false;
        }
    }
}

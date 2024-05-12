using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Lexi.Core.Api.Services.Foundations.TelegramHandles
{
    public partial class TelegramHandleService
    {
        private async ValueTask<bool> UserIsNull(
            ITelegramBotClient client,
            Update update,
            Models.Foundations.Users.User user)
        {
            if (user is null)
            {
                await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    replyMarkup: LevelMarkup(),
                    text: $"🎓LexiEnglishBot🎓\n\n" +
                    $"⚠️Welcome {update.Message.Chat.FirstName}, " +
                    $"you can test your English speaking skill.\n\n Choose your English level 🧠");

                storedTelegramId.Value = update.Message.Chat.Id;
                storedName.Value = update.Message.Chat.FirstName;
                telegramName.Value = update.Message.Chat.Username;

                await CreateExternalUserAsync();
                SetOrchestrationService(orchestrationService, update.Message.Chat.Id);

                return true;
            }

            return false;
        }
    }
}

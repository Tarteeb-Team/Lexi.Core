using Lexi.Core.Api.Models.Foundations.Users;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Lexi.Core.Api.Services.Foundations.TelegramHandles
{
    public partial class TelegramHandleService
    {
        private async ValueTask<bool> BackToMenu(
            ITelegramBotClient client,
            Update update,
            Models.Foundations.Users.User user)
        {
            if (update.Message.Text is "Menu 🎙")
            {
                await client.SendTextMessageAsync(
                       chatId: update.Message.Chat.Id,
                       replyMarkup: MenuMarkup(),
                       text: $"Choose 👇🏼");

                user.State = State.Active;
                await this.updateStorageBroker.UpdateUserAsync(user);

                return true;
            }

            return false;
        }
    }
}

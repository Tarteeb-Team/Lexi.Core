using Lexi.Core.Api.Models.Foundations.Users;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Lexi.Core.Api.Services.Foundations.TelegramHandles
{
    public partial class TelegramHandleService
    {
        private async ValueTask<bool> Settings(
            ITelegramBotClient client,
            Update update,
            Models.Foundations.Users.User user)
        {
            if (user.State is State.Active && update.Message.Text is "Settings ⚙️")
            {
                await client.SendTextMessageAsync(
                   chatId: update.Message.Chat.Id,
                   replyMarkup: SettingsMarkup(),
                   text: $"Settings 👇🏼");

                user.State = State.Settigns;
                await this.updateStorageBroker.UpdateUserAsync(user);

                return true;
            }

            return false;
        }
    }
}

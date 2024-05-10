using Lexi.Core.Api.Models.Foundations.Users;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Lexi.Core.Api.Brokers.TelegramBroker
{
    public partial class TelegramBroker
    {
        private async ValueTask<bool> PracticePartOne(
        ITelegramBotClient client,
        Update update,
        Models.Foundations.Users.User user)
        {
            if (user.State is State.Active && update.Message.Text is "Practice IELTS part 1 🎯")
            {


                return true;
            }

            return false;
        }
    }
}

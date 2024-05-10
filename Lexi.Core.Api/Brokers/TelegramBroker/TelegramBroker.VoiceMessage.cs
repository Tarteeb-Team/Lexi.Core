using Lexi.Core.Api.Models.Foundations.Users;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Lexi.Core.Api.Brokers.TelegramBroker
{
    public partial class TelegramBroker
    {
        private async ValueTask<bool> VoiceMessage(
            ITelegramBotClient client,
            Update update,
            Models.Foundations.Users.User user)
        {
            if (update.Message.Voice is not null && user.State is State.TestSpeechPronun)
            {
                var loadingMessage = await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: $"🎙️ Checking Pronunciation 🎙️\n\n" +
                          $"Loading...");

                messageId.Value = loadingMessage.MessageId;

                var file = await client.GetFileAsync(update.Message.Voice.FileId);

                using (var stream = new MemoryStream())
                {
                    await client.DownloadFileAsync(file.FilePath, stream);
                    stream.Position = 0;

                    ReturningConvertOggToWav(stream, update.Message.Chat.Id);
                }

                await CreateExternalUserAsync();

                SetOrchestrationService(orchestrationService, update.Message.Chat.Id);

                return true;
            }
            else if (user.State is State.TestSpeechPronun && update.Message.Voice is null)
            {
                await client.SendTextMessageAsync(
                      chatId: update.Message.Chat.Id,
                      text: $"🎓LexiEnglishBot🎓\n\n" +
                      $"Send only voice message, please 🙂");

                return true;
            }

            if (update.Message.Text is "/start")
            {
                await client.SendTextMessageAsync(
                   chatId: update.Message.Chat.Id,
                   replyMarkup: MenuMarkup(),
                   text: $"Choose 👇🏼");

                user.State = State.Active;
                await this.updateStorageBroker.UpdateUserAsync(user);

                return true;
            }
            else
            {
                await client.SendTextMessageAsync(
                      chatId: update.Message.Chat.Id,
                      text: $"🎓LexiEnglishBot🎓\n\n" +
                      $"Wrong choice 🙂");

                return true;
            }

            return false;
        }
    }
}

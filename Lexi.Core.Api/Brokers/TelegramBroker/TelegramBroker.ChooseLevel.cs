using Lexi.Core.Api.Models.Foundations.QuestionTypes;
using Lexi.Core.Api.Models.Foundations.Users;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Lexi.Core.Api.Brokers.TelegramBroker
{
    public partial class TelegramBroker
    {
        private async ValueTask<bool> ChooseLevel(
        ITelegramBotClient client,
        Update update,
        Models.Foundations.Users.User user)
        {
            if (user.State is State.Level)
            {
                if (update.Message.Text is "A1 😊"
                || update.Message.Text is "A2 😉"
                || update.Message.Text is "B1 😄"
                || update.Message.Text is "B2 😎"
                || update.Message.Text is "C1 😇"
                || update.Message.Text is "C2 🤗")
                {
                    storedLevel.Value = update.Message.Text;

                    await client.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        replyMarkup: VoiceMarkup(),
                        text: $"Your level is {update.Message.Text} ⭐️\n\nPlease choose your AI assistant's voice: 😊");

                    user.State = State.ChooseAIAssistantVoice;
                    user.Level = update.Message.Text;
                    await this.updateStorageBroker.UpdateUserAsync(user);

                    return true;
                }

                await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "Please, choose your level ❗️");

                return true;
            }
            if (user.State is State.ChooseAIAssistantVoice)
            {
                await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    replyMarkup: MenuMarkup(),
                    text: "👋 Welcome to our Lexi English Bot! 🤖\n\n" +
                    "Here, you can improve your English speaking and pronunciation. Our AI assistant will help you practice " +
                    "English speech and provide feedback to enhance your skills. " +
                    "Let's get started! 🚀\n\nFeel free to explore the menu options below:");

                var voiceType = new QuestionType
                {
                    Id = Guid.NewGuid(),
                    TelegramId = user.TelegramId
                };

                var voicetype = new QuestionType
                {
                    Id = Guid.NewGuid(),
                    TelegramId = user.TelegramId,
                };

                if (update.Message.Text == "Emma 🧑🏽‍🏫")
                    voicetype.Type = "en-US-EmmaNeural";

                if (update.Message.Text == "Brian 👨🏽‍🏫")
                    voicetype.Type = "en-US-AndrewNeural";

                await this.updateStorageBroker.InsertQuestionTypeAsync(voicetype);

                user.State = State.Active;
                await this.updateStorageBroker.UpdateUserAsync(user);

                return true;
            }

            return false;
        }
    }
}

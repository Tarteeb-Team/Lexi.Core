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
        private async ValueTask<bool> ChooseVoice(
            ITelegramBotClient client,
            Update update,
            Models.Foundations.Users.User user)
        {
            if (user.State is State.Settigns && update.Message.Text is "Voice 🗣️")
            {
                await client.SendTextMessageAsync(
                   chatId: update.Message.Chat.Id,
                   replyMarkup: VoiceMarkup(),
                   text: $"🎙️ Choose Your Teacher's Voice! 🎙️\r\n\r\nSelect a voice to personalize your learning experience:" +
                   $"\r\n\r\nEmma \U0001f9d1🏽‍🏫: " +
                   $"Clear and soothing voice.\r\n\nBrian 👨🏽‍🏫: " +
                   $"Energetic and engaging tone.\r\n\nPick your preference and let's get started! 😊🎶");

                user.State = State.ChooseVoice;
                await this.updateStorageBroker.UpdateUserAsync(user);

                return true;
            }
            if (user.State is State.ChooseVoice
                && (update.Message.Text == "Emma 🧑🏽‍🏫" || update.Message.Text == "Brian 👨🏽‍🏫"))
            {
                string chosenVoice = update.Message.Text;


                await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    replyMarkup: MenuMarkup(),
                    text: $"You've chosen {chosenVoice}! 🎉");

                var possibleVoiceType = this.updateStorageBroker
                    .SelectAllQuestionTypes().FirstOrDefault(q => q.TelegramId == user.TelegramId);

                if(possibleVoiceType is null)
                {
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
                }
                else
                {
                    if (update.Message.Text == "Emma 🧑🏽‍🏫")
                        possibleVoiceType.Type = "en-US-EmmaNeural";

                    if (update.Message.Text == "Brian 👨🏽‍🏫")
                        possibleVoiceType.Type = "en-US-AndrewNeural";

                    await this.updateStorageBroker.UpdateQuestionTypeAsync(possibleVoiceType);

                }

                user.State = State.Active;
                await this.updateStorageBroker.UpdateUserAsync(user);

                return true;
            }

            return false;
        }
    }
}

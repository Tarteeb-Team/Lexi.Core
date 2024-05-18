using Lexi.Core.Api.Models.Foundations.Questions;
using Lexi.Core.Api.Models.Foundations.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Lexi.Core.Api.Services.Foundations.TelegramHandles
{
    public partial class TelegramHandleService
    {
        private async ValueTask<bool> TestSpeechPronun(
        ITelegramBotClient client,
        Update update,
        Models.Foundations.Users.User user)
        {
            if (user.State is State.TestSpeech && update.Message.Text is "Test pronunciation 🎧")
            {
                await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    replyMarkup: PronunciationMarkup(),
                    text: $"🎧 Pronunciation test\r\n\r\n" +
                          $"You can:\r\n" +
                          $"1. Send a voice message 🎙 to check pronunciation and fluency. (~ 15 seconds)\r\n" +
                          $"2. Click 'Generate Question' if it's difficult to think of what to say.\r\n" +
                          $"\r\nI will evaluate your pronunciation and fluency based on your response. 😁");

                user.State = State.TestSpeechPronun;
                await this.updateStorageBroker.UpdateUserAsync(user);

                return true;
            }
            if (user.State is State.TestSpeechPronun && update.Message.Text == "Generate a question 🎁")
            {
                List<Question> questions = this.updateStorageBroker.SelectAllQuestions().ToList();

                if (questions.Count > 0) 
                {
                    var random = new Random();
                    var randomIndex = random.Next(0, questions.Count - 1);

                    var question = questions[randomIndex];
                    var questionText = question.Content;

                    await client.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        text: $"🎁 Here's your question: \n\n❓{questionText} \n\n" +
                              "🎙️ Now, it's your turn! Express yourself with a beautiful voice message. (~ 15 seconds)\n" +
                              "Let your pronunciation and fluency shine! 🌟");
                    return true;
                }
                else
                {
                    await client.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        text: "There are currently no questions available for speech pronunciation practice. Check back later or try a different section!"
                    );
                }
            }

            return false;
        }
    }
}

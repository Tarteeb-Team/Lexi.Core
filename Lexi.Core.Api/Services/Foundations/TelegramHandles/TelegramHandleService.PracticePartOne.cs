using Lexi.Core.Api.Models.Foundations.Questions;
using Lexi.Core.Api.Models.Foundations.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Lexi.Core.Api.Services.Foundations.TelegramHandles
{
    public partial class TelegramHandleService
    {
        private async ValueTask<bool> PracticePartOne(
            ITelegramBotClient client,
            Update update,
            Models.Foundations.Users.User user)
        {
            if (user.State is State.TestSpeech && update.Message.Text == "Practice IELTS part 1 🎯")
            {
                await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    replyMarkup: PartOneQuestionsMarkup(),
                    text: $"📚 Practice IELTS Part 1 📚\n\n" +
                          $"Choose a type of question to practice:");

                user.State = State.ChooseTypeOfQuestion;
                await this.updateStorageBroker.UpdateUserAsync(user);

                return true;
            }
            if (user.State is State.ChooseTypeOfQuestion && update.Message.Text == "Generate a question 🎁")
            {
                List<Question> questions = this.updateStorageBroker.SelectAllQuestions().ToList();
                var random = new Random();
                var randomIndex = random.Next(0, questions.Count);

                var question = questions[randomIndex];
                var questionText = question.Content;

                await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    replyMarkup: new ReplyKeyboardMarkup("Menu 🎙") { ResizeKeyboard = true },
                    text: $"🎯 Here's a crucial question for IELTS Part 1 ({question.QuestionType}):\n\n{question.Content}\n\n" +
                      "🎙️ Now, it's your turn! Take your time to articulate your thoughts clearly and confidently. " +
                      "Once you're ready, send a voice message with your response. 🌟");

                user.ImprovedSpeech = question.Content;

                user.State = State.PartOneTest;
                await this.updateStorageBroker.UpdateUserAsync(user);

                return true;
            }
            if (user.State is State.ChooseTypeOfQuestion &&
                this.updateStorageBroker.SelectAllQuestions()
                    .Any(q => q.QuestionType.Equals(update.Message.Text)))
            {
                var chosenType = update.Message.Text;

                var randomQuestion = this.updateStorageBroker.SelectAllQuestions()
                .Where(q => q.QuestionType.Equals(chosenType))
                .AsEnumerable()
                .OrderBy(_ => Guid.NewGuid())
                .FirstOrDefault();

                if (randomQuestion != null)
                {
                    await client.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        replyMarkup: new ReplyKeyboardMarkup("Menu 🎙") { ResizeKeyboard = true },
                        text: $"🎯 Here's a crucial question for IELTS Part 1 ({chosenType}):\n\n{randomQuestion.Content}\n\n" +
                          "🎙️ Now, it's your turn! Take your time to articulate your thoughts clearly and confidently. " +
                          "Once you're ready, send a voice message with your response. 🌟");

                    user.ImprovedSpeech = randomQuestion.Content;

                    user.State = State.PartOneTest;
                    await this.updateStorageBroker.UpdateUserAsync(user);

                    return true;
                }
                else
                {
                    await client.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        text: "No questions found for the selected type. Please choose another type.");

                    return false;
                }
            }

            return false;
        }
    }
}

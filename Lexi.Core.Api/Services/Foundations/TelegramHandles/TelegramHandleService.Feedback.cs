using Lexi.Core.Api.Models.Foundations.Reviews;
using Lexi.Core.Api.Models.Foundations.Users;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Lexi.Core.Api.Services.Foundations.TelegramHandles
{
    public partial class TelegramHandleService
    {
        private async ValueTask<bool> Feedback(
            ITelegramBotClient client,
            Update update,
            Models.Foundations.Users.User user)
        {
            if (user.State is State.Active && update.Message.Text is "Feedback 📝")
            {
                await client.SendTextMessageAsync(
                   chatId: update.Message.Chat.Id,
                   replyMarkup: FeedbackMarkup(),
                   text: $"Choose 👇🏼");

                user.State = State.Feedback;
                await this.updateStorageBroker.UpdateUserAsync(user);

                return true;
            }
            else if (user.State is State.Feedback && update.Message.Text is "Leave a review 📝")
            {
                await client.SendTextMessageAsync(
                   chatId: update.Message.Chat.Id,
                   replyMarkup: new ReplyKeyboardMarkup("Menu 🎙") { ResizeKeyboard = true},
                   text: $"Leave a review as text ✏️");

                user.State = State.LeaveReview;
                await this.updateStorageBroker.UpdateUserAsync(user);

                return true;
            }
            else if (user.State is State.LeaveReview)
            {
                await client.SendTextMessageAsync(
                   chatId: update.Message.Chat.Id,
                   replyMarkup: MenuMarkup(),
                   text: $"Thanks 😊");

                var review = new Review
                {
                    Id = Guid.NewGuid(),
                    Text = update.Message.Text,
                    TelegramId = user.TelegramId,
                    TelegramUserName = user.Name
                };

                await this.updateStorageBroker.InsertReviewAsync(review);

                user.State = State.Active;
                await this.updateStorageBroker.UpdateUserAsync(user);

                return true;
            }
            else if (user.State is State.Feedback && update.Message.Text is "View other reviews 🤯")
            {
                var reviews = this.updateStorageBroker.SelectAllReviews().ToList();
                int totalReviews = reviews.Count();
                int reviewsPerMessage = 80; 

                int partsCount = totalReviews / reviewsPerMessage + (totalReviews % reviewsPerMessage == 0 ? 0 : 1);

                for (int i = 0; i < partsCount; i++)
                {
                    int skipCount = i * reviewsPerMessage;
                    var reviewsInCurrentPart = reviews.Skip(skipCount).Take(reviewsPerMessage).ToList();
                    var stringBuilder = new StringBuilder();
                    stringBuilder.Append($"Reviews - Part {i + 1}/{partsCount}:\n\n");
                    int count = skipCount + 1;

                    foreach (var review in reviewsInCurrentPart)
                    {
                        stringBuilder.Append($"{count}. {review.TelegramUserName}: {review.Text}\n\n");
                        count++;
                    }

                    await client.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        replyMarkup: MenuMarkup(),
                        text: stringBuilder.ToString());

                    await Task.Delay(500);
                }

                user.State = State.Active;
                await this.updateStorageBroker.UpdateUserAsync(user);

                return true;
            }

            return false;
        }
    }
}

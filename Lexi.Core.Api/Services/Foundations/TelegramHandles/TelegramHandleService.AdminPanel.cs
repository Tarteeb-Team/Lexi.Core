﻿using System.Linq;
using System.Text;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Runtime.InteropServices;
using System.IO;

namespace Lexi.Core.Api.Services.Foundations.TelegramHandles
{
    public partial class TelegramHandleService
    {
        private async ValueTask<bool> AdminPanel(
            ITelegramBotClient client,
            Update update,
            Models.Foundations.Users.User user)
        {
            if (update.Message.Text.StartsWith("delete-"))
            {
                string reviewText = update.Message.Text.Substring(7);

                var review = this.updateStorageBroker
                    .SelectAllReviews().FirstOrDefault(r => r.Text == reviewText);
                if (review is not null)
                {
                    await this.updateStorageBroker.DeleteReviewAsync(review);

                    await client.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        text: $"Deleted");

                    return true;
                }
                else
                {
                    await client.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        text: $"Not found");

                    return true;
                }
            }
            if (update.Message.Text.StartsWith("deleteuser-"))
            {
                string telegramName = update.Message.Text.Substring(12);

                var possibleUser = this.updateStorageBroker
                    .SelectAllUsers().FirstOrDefault(u => u.TelegramName == telegramName);

                await this.updateStorageBroker.DeleteUserAsync(possibleUser);

                await client.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        text: $"Deleted: {possibleUser.TelegramName}");

                return true;
            }

            if (update.Message.Text == "/shpion")
            {
                var shpionMarkup = ShpionMarkup();

                await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    replyMarkup: shpionMarkup,
                    text: $"Salom shpion.");

                return true;
            }
            else if (update.Message.Text == "Count of users")
            {
                int count = 0;
                var allUser = this.updateStorageBroker.SelectAllUsers();

                foreach (var u in allUser)
                {
                    count++;
                }

                await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: $"Count: {count}");

                return true;
            }
            else if (update.Message.Text == "All users")
            {
                IQueryable<Models.Foundations.Users.User> allUsers = this.updateStorageBroker.SelectAllUsers();
                int totalUsers = allUsers.Count();
                int usersPerMessage = 50; // Adjusted to 50 users per message

                int partsCount = totalUsers / usersPerMessage + (totalUsers % usersPerMessage == 0 ? 0 : 1);

                for (int i = 0; i < partsCount; i++)
                {
                    int skipCount = i * usersPerMessage;

                    var usersInCurrentPart = allUsers.Skip(skipCount).Take(usersPerMessage).ToList();

                    var stringBuilder = new StringBuilder();
                    stringBuilder.Append($"All users - Part {i + 1}/{partsCount}:\n\n");

                    foreach (var user1 in usersInCurrentPart)
                    {
                        var voice = this.updateStorageBroker.SelectAllQuestionTypes()
                            .FirstOrDefault(v => v.TelegramId == user1.TelegramId);

                        stringBuilder.Append($"{skipCount + 1}. {user1.Name} " +
                            $"| @{user1.TelegramName} | {user1.Level} | {user1.State} | {voice.Type}\n\n");
                        skipCount++;
                    }

                    await client.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        text: stringBuilder.ToString());

                    await Task.Delay(500);
                }

                return true;
            }

            else if (update.Message.Text == "Admin tools")
            {
                await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "Tools:\n\n/notifyall - Notify all users to proceed using the bot\n\n" +
                    "/notifyerror - Notify all users if error is occured\n\n" +
                    "/notifygood - Notify all users ofter error\n\n" +
                    "/notifyallreview - Notify all users to leave review\n\n" +
                    "/notify-@(userName) - Notify specific user to proceed using the bot" +
                    "\n\n/time - Shows time\n\n" +
                    "notificationtime - Auto notitfication info" +
                    "\n\ndelete-reviewText - Delete review of a user");

                return true;
            }

            else if (update.Message.Text == "/notifyall")
            {
                await NotifyAllUsersAsync();

                await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "Notification sent to all users successfully!");

                return true;
            }
            else if (update.Message.Text == "/notifyerror")
            {
                await NotifyAllUsersErrorAsync();

                await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "Notification sent to all users successfully!");

                return true;
            }
            else if (update.Message.Text == "/notifyrelease")
            {
                await NotifyAllUsersReleaseAsync();

                await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "Notification sent to all users successfully!");

                return true;
            }
            else if (update.Message.Text == "/wordsnotification")
            {
                await GetAndSendRandomWords(7);

                await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "Notification sent to all users successfully!");

                return true;
            }
            else if (update.Message.Text == "/writeusers")
            {
                var users = this.updateStorageBroker.SelectAllUsers().ToList();

                string filePath = "users.txt";

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var i in users)
                    {
                        writer.WriteLine($"ID: {i.Id}, " +
                                         $"TelegramId: {i.TelegramId}, " +
                                         $"TelegramName: {i.TelegramName}, " +
                                         $"Level: {i.Level}, " +
                                         $"State: {i.State}, " +
                                         $"Name: {i.Name}, " +
                                         $"ImprovedSpeech: {i.ImprovedSpeech}, " +
                                         $"Overall: {user.Overall}");

                    }
                }


                using (var fileStream = System.IO.File.OpenRead(filePath))
                {
                    var fileToSend = InputFile.FromStream(fileStream, "users.txt");
                    await client.SendDocumentAsync(
                        chatId: update.Message.Chat.Id,
                        document: fileToSend,
                        caption: "Here is the list of users."
                    );
                }

                return true;
            }
            else if (update.Message.Text == "/writereviews")
            {
                System.Collections.Generic.List<Models.Foundations.Reviews.Review> reviews = this.updateStorageBroker.SelectAllReviews().ToList();

                string filePath = "reveiws.txt";

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var review in reviews)
                    {
                        writer.WriteLine($"ID: {review.Id}, " +
                                         $"TelegramId: {review.TelegramId}, " +
                                         $"TelegramUserName: {review.TelegramUserName}, " +
                                         $"Text: {review.Text}");
                    }
                }


                using (var fileStream = System.IO.File.OpenRead(filePath))
                {
                    var fileToSend = InputFile.FromStream(fileStream, "users.txt");
                    await client.SendDocumentAsync(
                        chatId: update.Message.Chat.Id,
                        document: fileToSend,
                        caption: "Here is the list of users."
                    );
                }

                return true;
            }
            else if (update.Message.Text == "/writequestions")
            {
                // Retrieve questions from the storage broker
                System.Collections.Generic.List<Models.Foundations.Questions.Question> questions = this.updateStorageBroker.SelectAllQuestions().ToList();

                string filePath = "questions.txt";

                // Write questions to a text file
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var question in questions)
                    {
                        writer.WriteLine($"ID: {question.Id}, " +
                                         $"Content: {question.Content}, " +
                                         $"Number: {question.Number}, " +
                                         $"QuestionType: {question.QuestionType}");
                    }
                }

                // Send the document containing questions
                using (var fileStream = System.IO.File.OpenRead(filePath))
                {
                    var fileToSend = InputFile.FromStream(fileStream, "questions.txt");
                    await client.SendDocumentAsync(
                        chatId: update.Message.Chat.Id,
                        document: fileToSend,
                        caption: "Here is the list of questions."
                    );
                }

                return true;
            }

            else if (update.Message.Text == "/notifygood")
            {
                await NotifyAllUsersGoodAsync();

                await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "Notification sent to all users successfully!");

                return true;
            }
            else if (update.Message.Text == "/active")
            {
                await ChangeUsersStatusToActiveAsync();

                await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "done!");

                return true;
            }
            else if (update.Message.Text == "/notifyallreview")
            {
                await NotifyUsersWithoutReviewAsync();

                await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "Notification sent to all users successfully!");

                return true;
            }
            else if (update.Message.Text.StartsWith("/notify-@"))
            {
                string userTelegramName = update.Message.Text.Substring(8);

                var userToSend = this.updateStorageBroker.SelectAllUsers()
                    .FirstOrDefault(u => u.TelegramName == userTelegramName);

                await SendReviewReminder(userToSend);
                await SendDailyNotification(user);

                await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: $"Notification sent to @{userTelegramName} users successfully!");

                return true;
            }
            else if (update.Message.Text == "/time")
            {
                var currentTime = DateTime.Now.AddHours(5);

                string formattedTime = currentTime.ToString("HH:mm:ss");

                await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: $"Current time: {formattedTime}");

                return true;
            }
            else if (update.Message.Text == "/notificationtime")
            {
                TimeSpan timeLeft = TimeSpan.FromMilliseconds(dailyNotificationTimer.Interval) - (DateTime.Now - lastNotificationTime);

                string timeLeftFormatted = $"{timeLeft.Hours} hours, {timeLeft.Minutes} minutes, and {timeLeft.Seconds} seconds";

                string formattedTime = DateTime.Now.ToString("HH:mm:ss");

                await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: $"Time left until next notification: {timeLeftFormatted}\n\nLast notification time: " +
                    $"{lastNotificationTime.AddHours(5).ToString("HH:mm:ss")}");

                return true;
            }

            return false;
        }
    }
}

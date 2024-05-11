using System.Linq;
using System.Text;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Runtime.InteropServices;

namespace Lexi.Core.Api.Brokers.TelegramBroker
{
    public partial class TelegramBroker
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
                int usersPerMessage = 80;

                int partsCount = totalUsers / usersPerMessage + (totalUsers % usersPerMessage == 0 ? 0 : 1);

                for (int i = 0; i < partsCount; i++)
                {
                    int skipCount = i * usersPerMessage;

                    var usersInCurrentPart = allUsers.Skip(skipCount).Take(usersPerMessage).ToList();

                    var stringBuilder = new StringBuilder();
                    stringBuilder.Append($"All users - Part {i + 1}/{partsCount}:\n\n");

                    foreach (var user1 in usersInCurrentPart)
                    {
                        stringBuilder.Append($"{skipCount + 1}. {user1.Name} | @{user1.TelegramName} | {user1.Level}\n\n");
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
                    "\n\n/notificationtime - Auto notitfication info" +
                    "delete-reviewText - Delete review of a user");

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
            else if (update.Message.Text == "/notifygood")
            {
                await NotifyAllUsersGoodAsync();

                await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "Notification sent to all users successfully!");

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

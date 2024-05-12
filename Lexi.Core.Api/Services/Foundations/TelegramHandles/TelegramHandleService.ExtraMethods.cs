using Concentus.Oggfile;
using Concentus.Structs;
using Lexi.Core.Api.Models.Foundations.ExternalUsers;
using Lexi.Core.Api.Services.Orchestrations;
using NAudio.Wave;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using System;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Lexi.Core.Api.Services.Foundations.TelegramHandles
{
    public partial class TelegramHandleService
    {
        public ValueTask<ExternalUser> CreateExternalUserAsync()
        {
            var externalUser = new ExternalUser();

            externalUser.TelegramId = storedTelegramId.Value;
            externalUser.Name = storedName.Value;
            externalUser.TelegramName = telegramName.Value;
            externalUser.Level = storedLevel.Value;
            externalUser.Id = Guid.NewGuid();

            return new ValueTask<ExternalUser>(externalUser);
        }

        public async Task SendTextMessageAsync(long chatId, string text)
        {
            await botClient.DeleteMessageAsync(chatId: chatId, messageId: messageId.Value);

            string wwwRootPath = Environment.CurrentDirectory;
            string audioDirectory = Path.Combine(wwwRootPath, "wwwroot", "AiVoices");

            string filePath = Path.Combine(audioDirectory, $"{chatId}.wav");

            var user = this.updateStorageBroker.SelectAllUsers().FirstOrDefault(u => u.TelegramId == chatId);

            if (System.IO.File.Exists(filePath))
            {
                using (var fileStream = System.IO.File.OpenRead(filePath))
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: text);

                    await botClient.SendVoiceAsync(
                        chatId: chatId,
                        caption: $"🎙️ Native Speech 🎙️\n\n" +
                                 $"Listen to native-like pronunciation:\n\n{user.ImprovedSpeech} 😁",
                        voice: InputFile.FromStream(fileStream));
                }
            }

            try
            {
                System.IO.File.Delete(filePath);

                if (Directory.Exists(audioDirectory))
                {
                    foreach (string filePath1 in Directory.GetFiles(audioDirectory, "*.wav"))
                    {
                        System.IO.File.Delete(filePath1);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        static async Task SendRequest(TelegramBotClient telegramBotClient)
        {
            try
            {
                using var httpClient = new HttpClient();

                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(
                    "https://lexicoreapi20240506000549.azurewebsites.net/api/Home");

                Console.WriteLine(httpResponseMessage.StatusCode);

                await telegramBotClient
                    .SendTextMessageAsync(
                    1924521160,
                    $"Request has sent.\nStatus: {httpResponseMessage.StatusCode}");
            }
            catch (Exception ex)
            {
                await telegramBotClient
                    .SendTextMessageAsync(1924521160, $"Error: {ex.Message}");

                return;
            }
        }

        public void ReturningConvertOggToWav(Stream stream, long userId)
        {
            using (MemoryStream pcmStream = new MemoryStream())
            {
                OpusDecoder decoder = OpusDecoder.Create(48000, 1);
                OpusOggReadStream oggIn = new OpusOggReadStream(decoder, stream);
                while (oggIn.HasNextPacket)
                {
                    short[] packet = oggIn.DecodeNextPacket();
                    if (packet != null)
                    {
                        foreach (short sample in packet)
                        {
                            var bytes = BitConverter.GetBytes(sample);
                            pcmStream.Write(bytes, 0, bytes.Length);
                        }
                    }
                }

                pcmStream.Position = 0;
                var wavStream = new RawSourceWaveStream(pcmStream, new WaveFormat(48000, 1));
                var sampleProvider = wavStream.ToSampleProvider();
                userPath = filePath + userId.ToString() + ".wav";

                WaveFileWriter.CreateWaveFile16(userPath, sampleProvider);
            }
        }
        
        public string ReturningConvertOggToWavSecond(Stream stream, long userId)
        {
            using (MemoryStream pcmStream = new MemoryStream())
            {
                OpusDecoder decoder = OpusDecoder.Create(48000, 1);
                OpusOggReadStream oggIn = new OpusOggReadStream(decoder, stream);
                while (oggIn.HasNextPacket)
                {
                    short[] packet = oggIn.DecodeNextPacket();
                    if (packet != null)
                    {
                        foreach (short sample in packet)
                        {
                            var bytes = BitConverter.GetBytes(sample);
                            pcmStream.Write(bytes, 0, bytes.Length);
                        }
                    }
                }

                pcmStream.Position = 0;
                var wavStream = new RawSourceWaveStream(pcmStream, new WaveFormat(48000, 1));
                var sampleProvider = wavStream.ToSampleProvider();
                userPath2 = filePath2 + userId.ToString() + ".wav";

                WaveFileWriter.CreateWaveFile16(userPath2, sampleProvider);

                return userPath2;
            }
        }

        public async Task NotifyAllUsersAsync()
        {
            try
            {
                var allUsers = updateStorageBroker.SelectAllUsers();

                foreach (var user in allUsers)
                {
                    await SendDailyNotification(user);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending daily notifications: {ex.Message}");
            }
        }

        public async Task NotifyAllUsersErrorAsync()
        {
            try
            {
                var allUsers = updateStorageBroker.SelectAllUsers();

                foreach (var user in allUsers)
                {
                    await NotificationError(user);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending daily notifications: {ex.Message}");
            }
        }

        public async Task NotifyAllUsersGoodAsync()
        {
            try
            {
                var allUsers = updateStorageBroker.SelectAllUsers();

                foreach (var user in allUsers)
                {
                    await NotificationGood(user);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending daily notifications: {ex.Message}");
            }
        }

        public async Task NotifyUsersWithoutReviewAsync()
        {
            try
            {
                var usersWithoutReview = updateStorageBroker.RetrieveUserWithoudReveiw();

                foreach (var user in usersWithoutReview)
                {
                    await SendReviewReminder(user);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending review reminders: {ex.Message}");
            }
        }

        private async void DailyNotificationTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var allUsers = updateStorageBroker.SelectAllUsers();

                foreach (var user in allUsers)
                {
                    await SendDailyNotification(user);
                }

                await Task.Delay(100000);

                var usersWithoutReview = updateStorageBroker.RetrieveUserWithoudReveiw();

                foreach (var user in usersWithoutReview)
                {
                    await SendReviewReminder(user);
                }

                lastNotificationTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending daily notifications: {ex.Message}");
            }
        }

        private async Task SendReviewReminder(
            Lexi.Core.Api.Models.Foundations.Users.User user)
        {
            try
            {
                string reminderMessage = $"❗️ Don't forget to leave a review for LexiEnglishBot!\n" +
                $"Your feedback helps us improve and serve you better.\n" +
                $"To leave a review, go to the 'Feedback 📝' -> 'Leave a review. 📝'\n\nThank you! ☺️";

                await botClient.SendTextMessageAsync(
                    chatId: user.TelegramId,
                    text: reminderMessage);
            }
            catch (Exception)
            {
                return;
            }

        }

        private async Task NotificationError(Lexi.Core.Api.Models.Foundations.Users.User user)
        {
            try
            {
                string notificationMessage = $"⚠️ Dear {user.Name},\n\nWe apologize for the inconvenience, but there seems to be an issue with our application. " +
                    $"Our team is working to resolve it as soon as possible. " +
                    $"Please try again later. Thank you for your patience and understanding. 😃";

                await botClient.SendTextMessageAsync(
                    chatId: user.TelegramId,
                    text: notificationMessage);
            }
            catch (Exception)
            {
                return;
            }
        }

        private async Task NotificationGood(Lexi.Core.Api.Models.Foundations.Users.User user)
        {
            try
            {
                string notificationMessage = $"🎉 Congratulations, {user.Name}!\n\nYou can now proceed using our application. " +
                    $"Thank you for your patience and understanding. Keep up the great work! 👍";

                await botClient.SendTextMessageAsync(
                    chatId: user.TelegramId,
                    text: notificationMessage);
            }
            catch (Exception)
            {
                return;
            }
        }



        private async Task SendDailyNotification(Lexi.Core.Api.Models.Foundations.Users.User user)
        {
            if (user.TelegramId is not 0)
            {
                try
                {
                    var currentTime = DateTime.Now.AddHours(5);
                    string timeOfDayMessage;
                    string message;
                    string emoji;

                    if (currentTime.Hour >= 5 && currentTime.Hour < 10)
                    {
                        timeOfDayMessage = "Good morning";
                        message = "Start your day with a positive attitude and practice your English skills!";
                        emoji = "☀️";
                    }
                    else if (currentTime.Hour >= 10 && currentTime.Hour < 18)
                    {
                        timeOfDayMessage = "Good day";
                        message = "Take advantage of the daylight hours to enhance your English proficiency!";
                        emoji = "✨";
                    }
                    else if (currentTime.Hour >= 18 && currentTime.Hour < 22)
                    {
                        timeOfDayMessage = "Good evening";
                        message = "Relax and unwind with some English practice before the night ends!";
                        emoji = "🌙";
                    }
                    else
                    {
                        timeOfDayMessage = "Good night";
                        message = "Reflect on your day and improve your English skills before bedtime!";
                        emoji = "🌃";
                    }

                    string notificationMessage = $"{emoji} {timeOfDayMessage}, {user.Name}!\n\n" +
                        $"{message}\n\n" +
                        "💬 Practice makes perfect! Take a few minutes today to speak English, whether it's with friends, " +
                        "practicing with our bot, or even speaking to yourself.\n\n" +
                        "🚀 Keep up the great work and strive for progress, not perfection!\n\n" +
                        "🎯 Remember, consistent effort leads to remarkable results!\n\n" +
                        "Keep shining bright! 💫";

                    await botClient.SendTextMessageAsync(
                        chatId: user.TelegramId,
                        text: notificationMessage);
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        public string ReturnFilePath()
        {
            return userPath;
        }

        public void SetOrchestrationService(
            IOrchestrationService orchestrationService, long telegramId = 0)
        {
            this.orchestrationService = orchestrationService;
            this.orchestrationService.GenerateSpeechFeedbackForUser(telegramId);
        }
    }
}

//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Concentus.Oggfile;
using Concentus.Structs;
using Lexi.Core.Api.Brokers.Storages;
using Lexi.Core.Api.Models.Foundations.ExternalUsers;
using Lexi.Core.Api.Models.Foundations.Reviews;
using Lexi.Core.Api.Models.Foundations.Users;
using Lexi.Core.Api.Services.Foundations.ImproveSpeech;
using Lexi.Core.Api.Services.Foundations.Users;
using Lexi.Core.Api.Services.Orchestrations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.OpenApi.Any;
using NAudio.Wave;
using RESTFulSense.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Lexi.Core.Api.Brokers.TelegramBroker
{
    public class TelegramBroker : ITelegramBroker
    {

        private TelegramBotClient botClient;
        private IOrchestrationService orchestrationService;
        private readonly IUserService userService;
        private readonly IOpenAIService openAIService;
        private readonly IStorageBroker storageBroker;
        private readonly IWebHostEnvironment _hostingEnvironment;

        private static readonly AsyncLocal<long> storedTelegramId = new AsyncLocal<long>();
        private static readonly AsyncLocal<int> messageId = new AsyncLocal<int>();
        private static readonly AsyncLocal<string> telegramName = new AsyncLocal<string>();
        private static readonly AsyncLocal<string> storedName = new AsyncLocal<string>();
        private static readonly AsyncLocal<string> storedLevel = new AsyncLocal<string>();
        private readonly System.Timers.Timer dailyNotificationTimer;
        private string filePath;
        private string userPath;

        public TelegramBroker(
            IServiceProvider serviceProvider,
            IUserService userService,
            IWebHostEnvironment hostingEnvironment,
            IOpenAIService openAIService,
            IStorageBroker storageBroker)
        {
            var token = "6866377621:AAFXOtQF6A4sP_L7tqn4C2DLqHqMie8KQ5k";
            this.botClient = new TelegramBotClient(token);
            this.userService = userService;
            this._hostingEnvironment = hostingEnvironment;
            filePath = Path.Combine(this._hostingEnvironment.WebRootPath, "outputWavs/");
            userPath = null;
            this.openAIService = openAIService;
            this.storageBroker = storageBroker;

            dailyNotificationTimer = new System.Timers.Timer
            {
                Interval = TimeSpan.FromHours(24).TotalMilliseconds,
                AutoReset = true, 
                Enabled = true
            };

            dailyNotificationTimer.Elapsed += DailyNotificationTimerElapsed;
        }

        public void StartListening()
        {
            dailyNotificationTimer.Start();

            botClient.StartReceiving(MessageHandler, ErrorHandler);
        }

        public async Task MessageHandler(ITelegramBotClient client, Update update, CancellationToken token)
        {
            try
            {
                if (update == null)
                {
                    return;
                }
                if (update.Message == null)
                {
                    return;
                }

                using var httpClient = new HttpClient();

                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(
                    "https://lexicoreapi20240506000549.azurewebsites.net/api/Home",
                    token);

                Console.WriteLine(httpResponseMessage.StatusCode);

                var user = this.userService
                     .RetrieveAllUsers().FirstOrDefault(u => u.TelegramId == update.Message.Chat.Id);

                if (user is null)
                {
                    await client.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        replyMarkup: LevelMarkup(),
                        text: $"🎓LexiEnglishBot🎓\n\n" +
                        $"⚠️Welcome {update.Message.Chat.FirstName}, " +
                        $"you can test your English speaking skill.\n\n Choose your English level 🧠");

                    storedTelegramId.Value = update.Message.Chat.Id;
                    storedName.Value = update.Message.Chat.FirstName;
                    telegramName.Value = update.Message.Chat.Username;

                    await CreateExternalUserAsync();

                    SetOrchestrationService(orchestrationService, update.Message.Chat.Id);

                    return;
                }

                else
                {
                    if (update.Message.Text is not null)
                    {
                        if (update.Message.Text.StartsWith("user-@"))
                        {
                            try
                            {
                                string username = update.Message.Text.Substring(6);

                                var persistedUser = this.userService.RetrieveAllUsers().FirstOrDefault(u => u.TelegramName == username);

                                string wwwRootPath = Environment.CurrentDirectory;
                                string filePath = Path.Combine(wwwRootPath, "wwwroot", "outputWavs", $"{persistedUser.TelegramId}.wav");

                                if (System.IO.File.Exists(filePath))
                                {
                                    using (var fileStream = System.IO.File.OpenRead(filePath))
                                    {
                                        await botClient.SendVoiceAsync(
                                        chatId: update.Message.Chat.Id,
                                        caption: $"{persistedUser.Name} | @{persistedUser.TelegramName}",
                                        voice: InputFile.FromStream(fileStream));
                                    }

                                    return;
                                }
                                else
                                {
                                    await client.SendTextMessageAsync(
                                        chatId: update.Message.Chat.Id,
                                        text: "Sorry, the audio file for this user does not exist.");

                                    return;
                                }
                            }
                            catch (Exception ex)
                            {
                                await client.SendTextMessageAsync(
                                        chatId: 1924521160,
                                        text: $"Error: {ex.Message}");

                                return;
                            }

                        }
                        if(update.Message.Text.StartsWith("delete-"))
                        {
                            string reviewText = update.Message.Text.Substring(7);

                            var review = this.storageBroker
                                .SelectAllReviews().FirstOrDefault(r => r.Text == reviewText);
                            if(review is not null)
                            {
                                await this.storageBroker.DeleteReviewAsync(review);

                                await client.SendTextMessageAsync(
                                    chatId: update.Message.Chat.Id,
                                    text: $"Deleted");

                                return;
                            }
                            else
                            {
                                await client.SendTextMessageAsync(
                                    chatId: update.Message.Chat.Id,
                                    text: $"Not found");

                                return;
                            }
                        }

                        if (update.Message.Text == "/shpion")
                        {
                            var shpionMarkup = ShpionMarkup();

                            await client.SendTextMessageAsync(
                                chatId: update.Message.Chat.Id,
                                replyMarkup: shpionMarkup,
                                text: $"Salom shpion.");

                            return;
                        }
                        else if (update.Message.Text == "Count of users")
                        {
                            int count = 0;
                            var allUser = this.userService.RetrieveAllUsers();

                            foreach (var u in allUser)
                            {
                                count++;
                            }

                            await client.SendTextMessageAsync(
                                chatId: update.Message.Chat.Id,
                                text: $"Count: {count}");

                            return;
                        }
                        else if (update.Message.Text == "All users")
                        {
                            int count = 1;
                            var allUser = this.userService.RetrieveAllUsers();

                            var stringBuilder = new StringBuilder();

                            stringBuilder.Append("All users:\n\n");

                            foreach (var u in allUser)
                            {
                                stringBuilder.Append($"{count}. {u.Name} | @{u.TelegramName} | {u.Level}\n\n");
                                count++;
                            }

                            stringBuilder.Append($"\nCount: {count - 1}");

                            await client.SendTextMessageAsync(
                                chatId: update.Message.Chat.Id,
                                text: stringBuilder.ToString());

                            return;
                        }
                        else if (update.Message.Text == "/notifyall")
                        {
                            await NotifyAllUsersAsync();

                            await client.SendTextMessageAsync(
                                chatId: update.Message.Chat.Id,
                                text: "Notification sent to all users successfully!");

                            return;
                        }
                        else if (update.Message.Text == "/notifyallreview")
                        {
                            await NotifyUsersWithoutReviewAsync();

                            await client.SendTextMessageAsync(
                                chatId: update.Message.Chat.Id,
                                text: "Notification sent to all users successfully!");

                            return;
                        }

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
                                   replyMarkup: MenuMarkup(),
                                   text: $"Welcome {user.Name} ⚡️\n\nYour level is {update.Message.Text} ⭐️\n\nChoose 👇🏼");

                                user.State = State.Active;
                                user.Level = update.Message.Text;
                                await this.userService.ModifyUserAsync(user);

                                return;
                            }
                            else
                            {
                                await client.SendTextMessageAsync(
                                    chatId: update.Message.Chat.Id,
                                    text: "Please, choose your level ❗️");
                            }
                        }

                        if (user.State is State.Active && update.Message.Text is "Test speech 🎙")
                        {
                            var prompt = $"Generate one IELTS part 1 random question for speaking (1 - 10000 -> give me one from 10000 common), and return it me. (return only question)";

                            var question = await this.openAIService.AnalizeRequestAsync("Look!!!", prompt);

                            await client.SendTextMessageAsync(
                               chatId: update.Message.Chat.Id,
                               replyMarkup: new ReplyKeyboardRemove(),
                               text: $"🎓 LexiEnglishBot 🎓\r\n\r\n" +
                               $"Try to answer the question ❓ or simply send a voice message 🎙:" +
                               $"\r\n\r\nQuestion: {question}\n\nI will provide feedback based on your response 😁"); ;

                            user.State = State.TestSpeech;
                            await this.userService.ModifyUserAsync(user);

                            return;
                        }
                        else if (user.State is State.Active && update.Message.Text is "Me 👤")
                        {
                            await client.SendTextMessageAsync(
                               chatId: update.Message.Chat.Id,
                               replyMarkup: OptionMarkup(),
                               text: $"About me👤\n\nName: {user.Name}\nLevel: {user.Level}\nAverage speech result: {user.Overall}% 🧠");

                            user.State = State.Me;
                            await this.userService.ModifyUserAsync(user);

                            return;
                        }
                        else if (user.State is State.Active && update.Message.Text is "Feedback 📝")
                        {
                            await client.SendTextMessageAsync(
                               chatId: update.Message.Chat.Id,
                               replyMarkup: FeedbackMarkup(),
                               text: $"Choose 👇🏼");

                            user.State = State.Feedback;
                            await this.userService.ModifyUserAsync(user);

                            return;
                        }
                        else if (user.State is State.Feedback && update.Message.Text is "Leave a review 📝")
                        {
                            await client.SendTextMessageAsync(
                               chatId: update.Message.Chat.Id,
                               replyMarkup: new ReplyKeyboardRemove(),
                               text: $"Leave a review as text ✏️");

                            user.State = State.LeaveReview;
                            await this.userService.ModifyUserAsync(user);

                            return;
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

                            await this.storageBroker.InsertReviewAsync(review);

                            user.State = State.Active;
                            await this.userService.ModifyUserAsync(user);

                            return;
                        }
                        else if (user.State is State.Feedback && update.Message.Text is "View other reviews 🤯")
                        {
                            var reviews = this.storageBroker.SelectAllReviews();
                            var stringBuilder = new StringBuilder();
                            stringBuilder.Append("All reviews ✨\n\n");
                            int count = 1;

                            foreach (var review in reviews)
                            {
                                stringBuilder.Append($"{count}. {review.TelegramUserName}: {review.Text}\n");
                                count++;
                            }

                            stringBuilder.Append("\nAll reviews ✨");

                            await client.SendTextMessageAsync(
                               chatId: update.Message.Chat.Id,
                               replyMarkup: MenuMarkup(),
                               text: stringBuilder.ToString());

                            user.State = State.Active;
                            await this.userService.ModifyUserAsync(user);

                            return;
                        }
                        else if (user.State is State.Me && update.Message.Text is "Change English level \U0001f92f")
                        {
                            await client.SendTextMessageAsync(
                               chatId: update.Message.Chat.Id,
                               replyMarkup: OptionMarkup(),
                               text: $"Choose 👇🏼");

                            return;
                        }
                        else if (user.State is State.Me && update.Message.Text is "Menu 🎙")
                        {
                            await client.SendTextMessageAsync(
                                   chatId: update.Message.Chat.Id,
                                   replyMarkup: MenuMarkup(),
                                   text: $"Choose 👇🏼");

                            user.State = State.Active;
                            await this.userService.ModifyUserAsync(user);

                            return;
                        }
                        else if (user.State is State.Me && update.Message.Text is "Change my English level \U0001f92f")
                        {
                            await client.SendTextMessageAsync(
                                   chatId: update.Message.Chat.Id,
                                   replyMarkup: LevelMarkup(),
                                   text: $"Choose your current English level: 👇🏼");

                            user.State = State.ChangeLevel;
                            await this.userService.ModifyUserAsync(user);

                            return;
                        }
                        else if (user.State is State.ChangeLevel)
                        {
                            await client.SendTextMessageAsync(
                                   chatId: update.Message.Chat.Id,
                                   replyMarkup: MenuMarkup(),
                                   text: $"Changed 👍🏼");

                            user.State = State.Active;
                            user.Level = update.Message.Text;
                            await this.userService.ModifyUserAsync(user);

                            return;
                        }
                    }


                    if (update.Message.Text is "/start")
                    {
                        await client.SendTextMessageAsync(
                           chatId: update.Message.Chat.Id,
                           replyMarkup: MenuMarkup(),
                           text: $"Choose 👇🏼");

                        user.State = State.Active;
                        await this.userService.ModifyUserAsync(user);

                        return;
                    }

                    if (update.Message.Voice is not null && user.State is State.TestSpeech)
                    {
                        var loadingMessage = await client.SendTextMessageAsync(
                               chatId: update.Message.Chat.Id,
                               text: $"🎓LexiEnglishBot🎓\n\n" +
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
                    }
                    else
                    {
                        await client.SendTextMessageAsync(
                              chatId: update.Message.Chat.Id,
                              text: $"🎓LexiEnglishBot🎓\n\n" +
                              $"Ops. click /start 🙂");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                await client.SendTextMessageAsync(
                    chatId: 1924521160,
                    text: $"Error: {ex.Message}");

                return;
            }
           
        }
        private static ReplyKeyboardMarkup ShpionMarkup()
        {
            var keyboardButtons = new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("All users"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Count of users")
                }
            };

            return new ReplyKeyboardMarkup(keyboardButtons)
            {
                ResizeKeyboard = true
            };
        }

        private static ReplyKeyboardMarkup OptionMarkup()
        {
            var keyboardButtons = new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Menu 🎙")
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Change my English level 🤯"),
                }
            };

            return new ReplyKeyboardMarkup(keyboardButtons)
            {
                ResizeKeyboard = true
            };
        }
        private static ReplyKeyboardMarkup FeedbackMarkup()
        {
            var keyboardButtons = new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Leave a review 📝")
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("View other reviews 🤯"),
                }
            };

            return new ReplyKeyboardMarkup(keyboardButtons)
            {
                ResizeKeyboard = true
            };
        }

        private static ReplyKeyboardMarkup MenuMarkup()
        {
            var keyboardButtons = new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Test speech 🎙"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Me 👤"),
                    new KeyboardButton("Feedback 📝")
                }
            };

            return new ReplyKeyboardMarkup(keyboardButtons)
            {
                ResizeKeyboard = true
            };
        }

        private static ReplyKeyboardMarkup LevelMarkup()
        {
            var keyboardButtons = new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("A1 😊"),
                    new KeyboardButton("A2 😉"),
                    new KeyboardButton("B1 😄"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("B2 😎"),
                    new KeyboardButton("C1 😇"),
                    new KeyboardButton("C2 🤗"),
                }
            };

            return new ReplyKeyboardMarkup(keyboardButtons)
            {
                ResizeKeyboard = true
            };
        }


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
            string filePath = Path.Combine(wwwRootPath, "wwwroot", "AiVoices", $"{chatId}.wav");

            var user = this.userService.RetrieveAllUsers().FirstOrDefault(u => u.TelegramId == chatId);

            if (System.IO.File.Exists(filePath))
            {
                using (var fileStream = System.IO.File.OpenRead(filePath))
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        replyMarkup: MenuMarkup(),
                        text: text);

                    await botClient.SendVoiceAsync(
                         chatId: chatId,
                         caption: $"\n\nTry it like this 🎁\n\n{user.ImprovedSpeech} 😁",
                         voice: InputFile.FromStream(fileStream));

                    user.State = State.Active;
                    await this.userService.ModifyUserAsync(user);

                }
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

        public async Task NotifyAllUsersAsync()
        {
            try
            {
                var allUsers = userService.RetrieveAllUsers();

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

        public async Task NotifyUsersWithoutReviewAsync()
        {
            try
            {
                var usersWithoutReview = storageBroker.RetrieveUserWithoudReveiw();

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
                var allUsers = userService.RetrieveAllUsers();

                foreach (var user in allUsers)
                {
                    await SendDailyNotification(user);
                }

                var allReviews = this.storageBroker.SelectAllReviews();

                var usersWithoutReview = allUsers
                    .Where(u => !allReviews.Any(r => r.TelegramId == u.TelegramId));

                foreach (var user in usersWithoutReview)
                {
                    await SendReviewReminder(user);
                }
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

        private async Task SendDailyNotification(
            Lexi.Core.Api.Models.Foundations.Users.User user)
        {
            if(user.TelegramId is not 0)
            {
                try
                {
                    string notificationMessage = $"🌟 Good morning, {user.Name}!\n\n" +
                        "🗣️ It's a new day, which means it's another opportunity to improve your English speaking skills!\n\n" +
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

        static Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}

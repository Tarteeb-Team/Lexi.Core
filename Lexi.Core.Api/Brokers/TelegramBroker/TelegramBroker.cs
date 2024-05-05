//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using aisha_ai.Services.Foundations.HandleSpeeches;
using Concentus.Oggfile;
using Concentus.Structs;
using Lexi.Core.Api.Models.Foundations.ExternalUsers;
using Lexi.Core.Api.Services.Foundations.Users;
using Lexi.Core.Api.Services.Orchestrations;
using Microsoft.AspNetCore.Hosting;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly IWebHostEnvironment _hostingEnvironment;

        private static readonly AsyncLocal<long> storedTelegramId = new AsyncLocal<long>();
        private static readonly AsyncLocal<int> messageId = new AsyncLocal<int>();
        private static readonly AsyncLocal<string> telegramName = new AsyncLocal<string>();
        private static readonly AsyncLocal<string> storedName = new AsyncLocal<string>();
        private string filePath;
        private string userPath;


        public TelegramBroker(
            IServiceProvider serviceProvider,
            IUserService userService,
            IWebHostEnvironment hostingEnvironment)
        {
            var token = "6866377621:AAFXOtQF6A4sP_L7tqn4C2DLqHqMie8KQ5k";
            this.botClient = new TelegramBotClient(token);
            this.userService = userService;
            this._hostingEnvironment = hostingEnvironment;
            filePath = Path.Combine(this._hostingEnvironment.WebRootPath, "outputWavs/");
            userPath = null;
        }

        public void StartListening()
        {
            botClient.StartReceiving(MessageHandler, ErrorHandler);
        }

        public async Task MessageHandler(ITelegramBotClient client, Update update, CancellationToken token)
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
                "https://lexicoreapi20240505120625.azurewebsites.net/api/Feedback",
                token);

            Console.WriteLine(httpResponseMessage.StatusCode);

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
                        }
                        else
                        {
                            await client.SendTextMessageAsync(
                                chatId: update.Message.Chat.Id,
                                text: "Sorry, the audio file for this user does not exist.");
                        }

                        return;
                    }
                    catch (Exception ex)
                    {
                        await client.SendTextMessageAsync(
                                chatId: 1924521160,
                                text: $"Error: {ex.Message}");

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
                        stringBuilder.Append($"{count}. {u.Name} | @{u.TelegramName}\n\n");
                        count++;
                    }

                    stringBuilder.Append($"\nCount: {count - 1}");

                    await client.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        text: stringBuilder.ToString());

                    return;
                }

                var user = this.userService
                .RetrieveAllUsers().FirstOrDefault(u => u.TelegramId == update.Message.Chat.Id);

                if (user is null)
                {
                    await client.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        text: $"🎓LexiEnglishBot🎓\n\n" +
                        $"⚠️Welcome {update.Message.Chat.FirstName}, " +
                        $"you can test your English speaking skill.\n\n Send voice message🎙");
                }
                else
                {
                    await client.SendTextMessageAsync(
                       chatId: update.Message.Chat.Id,
                       text: $"🎓LexiEnglishBot🎓\n\n" +
                       $"Send voice message please🎙");
                }
            }
            else if (update.Message.Voice is not null)
            {
                var loadingMessage = await client.SendTextMessageAsync(
                       chatId: update.Message.Chat.Id,
                       text: $"🎓LexiEnglishBot🎓\n\n" +
                       $"Loading...");

                storedTelegramId.Value = update.Message.Chat.Id;
                storedName.Value = update.Message.Chat.FirstName;
                telegramName.Value = update.Message.Chat.Username;
                messageId.Value = loadingMessage.MessageId;

                var file = await client.GetFileAsync(update.Message.Voice.FileId);

                using (var stream = new MemoryStream())
                {
                    await client.DownloadFileAsync(file.FilePath, stream);
                    stream.Position = 0;

                    ReturningConvertOggToWav(stream, update.Message.Chat.Id);
                }
            }
            else
            {
                await client.SendTextMessageAsync(
                      chatId: update.Message.Chat.Id,
                      text: $"🎓LexiEnglishBot🎓\n\n" +
                      $"Send voice message please🎙");
                return;
            }

            await CreateExternalUserAsync();

            SetOrchestrationService(orchestrationService);
        }
        private static ReplyKeyboardMarkup ShpionMarkup()
        {
            var keyboardButtons = new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("All users"),
                    new KeyboardButton("Count of users")
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
            externalUser.Id = Guid.NewGuid();

            return new ValueTask<ExternalUser>(externalUser);
        }

        public async Task SendTextMessageAsync(long chatId, string text)
        {
            await botClient.DeleteMessageAsync(chatId: chatId, messageId: messageId.Value);

            string wwwRootPath = Environment.CurrentDirectory;
            string filePath = Path.Combine(wwwRootPath, "wwwroot", "AiVoices", $"{chatId}.wav");



            if (System.IO.File.Exists(filePath))
            {
                using (var fileStream = System.IO.File.OpenRead(filePath))
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: text);

                   await botClient.SendVoiceAsync(
                        chatId: chatId,
                        caption: "\n\nTry it like this 🎁",
                        voice: InputFile.FromStream(fileStream));
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

        public string ReturnFilePath()
        {
            return userPath;
        }

        public void SetOrchestrationService(IOrchestrationService orchestrationService)
        {
            this.orchestrationService = orchestrationService;
            this.orchestrationService.GenerateSpeechFeedbackForUser();
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

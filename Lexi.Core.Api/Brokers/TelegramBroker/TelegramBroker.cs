//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Concentus.Oggfile;
using Concentus.Structs;
using Lexi.Core.Api.Models.Foundations.ExternalUsers;
using Lexi.Core.Api.Services.Foundations.Users;
using Lexi.Core.Api.Services.Orchestrations;
using Microsoft.AspNetCore.Hosting;
using NAudio.Wave;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

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
        private static readonly AsyncLocal<string> storedName = new AsyncLocal<string>();
        private string filePath;



        public string _filePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "audio.wav");

        public TelegramBroker(IServiceProvider serviceProvider, IUserService userService, IWebHostEnvironment hostingEnvironment)
        {
            var token = "6866377621:AAFXOtQF6A4sP_L7tqn4C2DLqHqMie8KQ5k";
            this.botClient = new TelegramBotClient(token);
            this.userService = userService;
            this._hostingEnvironment = hostingEnvironment;
            string fileName = "output.wav";
            filePath = Path.Combine(this._hostingEnvironment.WebRootPath, fileName);
        }

        public void StartListening()
        {
            botClient.StartReceiving(MessageHandler, ErrorHandler);
        }

        public async Task MessageHandler(ITelegramBotClient client, Update update, CancellationToken token)
        {
            if (update.Message.Text is not null)
            {
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
                messageId.Value = loadingMessage.MessageId;

                var file = await client.GetFileAsync(update.Message.Voice.FileId);

                using (var stream = new MemoryStream())
                {
                    await client.DownloadFileAsync(file.FilePath, stream);
                    stream.Position = 0;

                    ReturningConvertOggToWav(stream);
                }
            }
            ReturnFilePath();

            await CreateExternalUserAsync();

            SetOrchestrationService(orchestrationService);
        }

        public ValueTask<ExternalUser> CreateExternalUserAsync()
        {
            var externalUser = new ExternalUser();

            externalUser.TelegramId = storedTelegramId.Value;
            externalUser.Name = storedName.Value;
            externalUser.Id = Guid.NewGuid();

            return new ValueTask<ExternalUser>(externalUser);
        }

        public async Task SendTextMessageAsync(long chatId, string text)
        {
            await botClient.DeleteMessageAsync(chatId: chatId, messageId: messageId.Value);
            await botClient.SendTextMessageAsync(chatId, text);
        }

        public void ReturningConvertOggToWav(Stream stream)
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

                WaveFileWriter.CreateWaveFile16(filePath, sampleProvider);
            }
        }

        public string ReturnFilePath()
        {
            return filePath;
        }

        public void SetOrchestrationService(IOrchestrationService orchestrationService)
        {
            this.orchestrationService = orchestrationService;

            this.orchestrationService.GenerateSpeechFeedbackForUser();
        }

        static async Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
        }
    }
}

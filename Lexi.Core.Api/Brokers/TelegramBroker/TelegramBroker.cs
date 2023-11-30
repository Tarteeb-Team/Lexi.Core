//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Azure.Core;
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
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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



        //public string _filePath { get; set; } =
        //    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "audio.wav");

        public TelegramBroker(IServiceProvider serviceProvider, IUserService userService, IWebHostEnvironment hostingEnvironment)
        {
            var token = "6778362040:AAG7McOhfZjr8HfiFb0oPM_a2qsizii4PJo";
            this.botClient = new TelegramBotClient(token);
            this.userService = userService;
            this._hostingEnvironment = hostingEnvironment;
        }


        public void StartListening()
        {
            botClient.StartReceiving(MessageHandler, ErrorHandler);
        }

        public async Task MessageHandler(ITelegramBotClient client, Update update, CancellationToken token)
        {
            
                //return;
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
            if (update.Message.Type == MessageType.Voice)
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


            string fileName = "output.wav";

            using (MemoryStream pcmStream = new MemoryStream())
            {
                OpusDecoder decoder = OpusDecoder.Create(48000, 1);
                OpusOggReadStream oggIn = new OpusOggReadStream(decoder, stream);
                while (oggIn.HasNextPacket)
                {
                    short[] packet = oggIn.DecodeNextPacket();
                    if (packet != null)
                    {
                        for (int i = 0; i < packet.Length; i++)
                        {
                            var bytes = BitConverter.GetBytes(packet[i]);
                            pcmStream.Write(bytes, 0, bytes.Length);
                        }
                    }
                }

                pcmStream.Position = 0;

                using (WaveFileWriter writer = new WaveFileWriter(fileName, new WaveFormat(48000, 1)))
                {
                    byte[] buffer = new byte[pcmStream.Length];
                    pcmStream.Read(buffer, 0, buffer.Length);
                    writer.Write(buffer, 0, buffer.Length);
                }
            }

            using (WebClient client = new WebClient())
            {
                client.Credentials = new NetworkCredential("xchangertest", "NikonD40+");
                client.UploadFile("ftp://files.000webhost.com/" + fileName, fileName);
            }
        }

        public string ReturnFilePath()
        {
            string ftpServerUrl = "ftp://xchangertest@files.000webhost.com";
            string username = "xchangertest";
            string password = "NikonD40+";
            string remoteFilePath = "/output.wav";
            string localFilePath = Path.Combine(_hostingEnvironment.WebRootPath, "output.wav");

            using (WebClient client = new WebClient())
            {
                client.Credentials = new NetworkCredential(username, password);
                client.DownloadFile($"{ftpServerUrl}{remoteFilePath}", localFilePath);
            }

            return localFilePath;
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

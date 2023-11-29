//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Concentus.Oggfile;
using Concentus.Structs;
using Lexi.Core.Api.Models.Foundations.ExternalUsers;
using Lexi.Core.Api.Services.Orchestrations;
using NAudio.Wave;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Lexi.Core.Api.Brokers.TelegramBroker
{
    public class TelegramBroker : ITelegramBroker
    {
        private TelegramBotClient botClient;
        private static readonly AsyncLocal<long> storedTelegramId = new AsyncLocal<long>();
        private static readonly AsyncLocal<string> storedName = new AsyncLocal<string>();
        private IOrchestrationService orchestrationService;

        public string _filePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "audio.wav");

        public TelegramBroker(IServiceProvider serviceProvider)
        {
            var token = "6730993098:AAGbcKM4YBFAkzav-RRoiqsuzNOySrMpeS0";
            this.botClient = new TelegramBotClient(token);
        }

        public void StartListening()
        {
            botClient.StartReceiving(MessageHandler, ErrorHandler);
        }

        public async Task MessageHandler(ITelegramBotClient client, Update update, CancellationToken token)
        {
            if (update.Message.Text is not null)
            {
                await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: $"🎓LexiEnglishBot🎓\n\n" +
                    $"⚠️Welcome {update.Message.Chat.FirstName}, " +
                    $"you can test your English speaking skill.\n\n Send voice message🎙");
            }
            else if (update.Message.Voice is not null)
            {
                var voiceMessage = update.Message.Voice;
                storedTelegramId.Value = update.Message.Chat.Id;
                storedName.Value = update.Message.Chat.FirstName;

                // Get file information
                var file = await client.GetFileAsync(voiceMessage.FileId);

                // Download the file and get the stream
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

        public string ReturnFilePath()
        {
            return _filePath;
        }

        public async Task SendTextMessageAsync(long chatId, string text)
        {
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
                        for (int i = 0; i < packet.Length; i++)
                        {
                            var bytes = BitConverter.GetBytes(packet[i]);
                            pcmStream.Write(bytes, 0, bytes.Length);
                        }
                    }
                }
                pcmStream.Position = 0;
                var wavStream = new RawSourceWaveStream(pcmStream, new WaveFormat(48000, 1));
                var sampleProvider = wavStream.ToSampleProvider();
                WaveFileWriter.CreateWaveFile16(_filePath, sampleProvider);
            }
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

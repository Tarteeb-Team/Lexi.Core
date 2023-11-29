﻿//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System.Threading.Tasks;
using System.Threading;
using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.IO;
using Concentus.Oggfile;
using Concentus.Structs;
using NAudio.Wave;
using Lexi.Core.Api.Models.Foundations.ExternalUsers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Lexi.Core.Api.Services.Orchestrations;

namespace Lexi.Core.Api.Brokers.TelegramBroker
{
    public class TelegramBroker : ITelegramBroker
    {
        private TelegramBotClient botClient;
        private long storedTelegramId;
        private string storedName;
        private readonly IServiceProvider serviceProvider;
        public string _filePath = 
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "audio.wav");

        public TelegramBroker(IServiceProvider serviceProvider)
        {
            var token = "6505501647:AAEefapD-rEHaoFw6gyG-UNbI3KCCm6NxKU";
            this.botClient = new TelegramBotClient(token);
            this.serviceProvider = serviceProvider;
        }

        public void StartListening()
        {
            botClient.StartReceiving(MessageHandler, ErrorHandler);
        }

        public async Task MessageHandler(ITelegramBotClient client, Update update, CancellationToken token)
        {
            if (update.Message.Voice is not null)
            {
                var voiceMessage = update.Message.Voice;

                storedTelegramId = update.Message.Chat.Id;
                storedName = update.Message.Chat.FirstName;

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

            var orchestrationService = serviceProvider.GetRequiredService<IOrchestrationService>();

            await orchestrationService.GenerateSpeechFeedbackForUser();
        }

        public ValueTask<ExternalUser> CreateExternalUserAsync()
        {
            var externalUser = new ExternalUser();

            externalUser.TelegramId = storedTelegramId;
            externalUser.Name = storedName;
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

        static async Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
        }
    }
}
//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Brokers.Speeches;
using Lexi.Core.Api.Brokers.UpdateStorages;
using Lexi.Core.Api.Services.Foundations.ImproveSpeech;
using Lexi.Core.Api.Services.Orchestrations;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace Lexi.Core.Api.Brokers.TelegramBroker
{
    public partial class TelegramBroker : ITelegramBroker
    {

        private TelegramBotClient botClient;
        private IOrchestrationService orchestrationService;
        private readonly IOpenAIService openAIService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IUpdateStorageBroker updateStorageBroker;
        private readonly ISpeechBroker speechBroker;
        private static readonly AsyncLocal<long> storedTelegramId = new AsyncLocal<long>();
        private static readonly AsyncLocal<int> messageId = new AsyncLocal<int>();
        private static readonly AsyncLocal<int> messageId2 = new AsyncLocal<int>();
        private static readonly AsyncLocal<string> telegramName = new AsyncLocal<string>();
        private static readonly AsyncLocal<string> storedName = new AsyncLocal<string>();
        private static readonly AsyncLocal<string> storedLevel = new AsyncLocal<string>();
        private readonly System.Timers.Timer dailyNotificationTimer;
        private readonly System.Timers.Timer requestTimer;
        private DateTime lastNotificationTime;
        private string filePath;
        private string filePath2;
        private string userPath;
        private string userPath2;

        public TelegramBroker(
            IWebHostEnvironment hostingEnvironment,
            IOpenAIService openAIService,
            IUpdateStorageBroker updateStorageBroker,
            ISpeechBroker speechBroker)
        {
            var token = "6908660319:AAE5I0sDaBLp5P5nm1Kf1ywdl7LmZXC-kqQ";
            this.botClient = new TelegramBotClient(token);
            this._hostingEnvironment = hostingEnvironment;
            filePath = Path.Combine(this._hostingEnvironment.WebRootPath, "outputWavs/");
            filePath2 = Path.Combine(this._hostingEnvironment.WebRootPath, "PartOneAnswers/");
            userPath = null;
            userPath2 = null;
            this.openAIService = openAIService;

            dailyNotificationTimer = new System.Timers.Timer
            {
                Interval = TimeSpan.FromHours(24).TotalMilliseconds,
                AutoReset = true,
                Enabled = true
            };

            dailyNotificationTimer.Elapsed += DailyNotificationTimerElapsed;
            lastNotificationTime = DateTime.Now;

            requestTimer = new System.Timers.Timer
            {
                Interval = TimeSpan.FromMinutes(10).TotalMilliseconds,
                AutoReset = true,
                Enabled = true
            };

            requestTimer.Elapsed += async (sender, e) => await SendRequest(this.botClient);
            this.updateStorageBroker = updateStorageBroker;
            this.speechBroker = speechBroker;
        }

        public async Task MessageHandler(ITelegramBotClient client, Update update, CancellationToken token)
        {
            try
            {
                var user = this.updateStorageBroker
                     .SelectAllUsers().FirstOrDefault(u => u.TelegramId == update.Message.Chat.Id);

                if (update.Message.Text is not null)
                {
                    if (await UserIsNull(client, update, user))
                        return;
                    if (await AdminPanel(client, update, user))
                        return;
                    if (await ChooseLevel(client, update, user))
                        return;
                    if (await BackToMenu(client, update, user))
                        return;
                    if (await TestSpeech(client, update, user))
                        return;
                    if (await TestSpeechPronun(client, update, user))
                        return;
                    if (await PracticePartOne(client, update, user))
                        return;
                    if (await Feedback(client, update, user))
                        return;
                    if (await Me(client, update, user))
                        return;
                }

                if (await VoiceMessage(client, update, user))
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

        public void StartListening()
        {
            requestTimer.Start();
            dailyNotificationTimer.Start();
            botClient.StartReceiving(MessageHandler, ErrorHandler);
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

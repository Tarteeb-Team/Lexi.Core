//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Brokers.Speeches;
using Lexi.Core.Api.Brokers.Telegrams;
using Lexi.Core.Api.Brokers.UpdateStorages;
using Lexi.Core.Api.Services.Foundations.ImproveSpeech;
using Lexi.Core.Api.Services.Orchestrations;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Lexi.Core.Api.Services.Foundations.TelegramHandles
{
    public partial class TelegramHandleService : ITelegramHandleService
    {

        private static Func<Update, ValueTask> taskHandler;
        private IOrchestrationService orchestrationService;
        private readonly IOpenAIService openAIService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IUpdateStorageBroker updateStorageBroker;
        private readonly ISpeechBroker speechBroker;
        private readonly ITelegramBroker telegramBroker;
        private readonly IWordsToLearn wordsToLearn;
        private readonly TelegramBotClient botClient;
        private static readonly AsyncLocal<long> storedTelegramId = new AsyncLocal<long>();
        private static readonly AsyncLocal<int> messageId = new AsyncLocal<int>();
        private static readonly AsyncLocal<int> messageId2 = new AsyncLocal<int>();
        private static readonly AsyncLocal<string> telegramName = new AsyncLocal<string>();
        private static readonly AsyncLocal<string> storedName = new AsyncLocal<string>();
        private static readonly AsyncLocal<string> storedLevel = new AsyncLocal<string>();
        private readonly System.Timers.Timer dailyNotificationTimer;
        private readonly System.Timers.Timer requestTimer;
        private readonly System.Timers.Timer newWordsTimer;
        private DateTime lastNotificationTime;
        private string filePath;
        private string filePath2;
        private string userPath;
        private string userPath2;

        public TelegramHandleService(
            IWebHostEnvironment hostingEnvironment,
            IOpenAIService openAIService,
            IUpdateStorageBroker updateStorageBroker,
            ISpeechBroker speechBroker,
            ITelegramBroker telegramBroker,
            IWordsToLearn wordsToLearn)
        {
            this._hostingEnvironment = hostingEnvironment;
            filePath = Path.Combine(this._hostingEnvironment.WebRootPath, "outputWavs/");
            filePath2 = Path.Combine(this._hostingEnvironment.WebRootPath, "PartOneAnswers/");
            userPath = null;
            userPath2 = null;
            this.openAIService = openAIService;

            dailyNotificationTimer = new System.Timers.Timer
            {
                Interval = TimeSpan.FromDays(7).TotalMilliseconds,
                AutoReset = true,
                Enabled = true
            };

            dailyNotificationTimer.Elapsed += DailyNotificationTimerElapsed;
            lastNotificationTime = DateTime.Now;

            requestTimer = new System.Timers.Timer
            {
                Interval = TimeSpan.FromDays(1).TotalMilliseconds,
                AutoReset = true,
                Enabled = true
            };

            requestTimer.Elapsed += async (sender, e) => await SendRequest(botClient);

            newWordsTimer = new System.Timers.Timer
            {
                Interval = TimeSpan.FromDays(3).TotalMilliseconds,
                AutoReset = true,
                Enabled = true
            };

            newWordsTimer.Elapsed += async (sender, e) => await GetAndSendRandomWords(7);

            this.updateStorageBroker = updateStorageBroker;
            this.speechBroker = speechBroker;
            this.telegramBroker = telegramBroker;
            botClient = this.telegramBroker.ReturnTelegramBotClient();

            requestTimer.Start();
            newWordsTimer.Start();
            dailyNotificationTimer.Start();
            this.wordsToLearn = wordsToLearn;
        }

        public void ListenTelegramUserMessage()
        {
            this.telegramBroker.RegisterTelegramEventHandler(async (update, client) =>
            {
                await this.ProcessMessageHandler(update, client);
            });
        }

        private async Task ProcessMessageHandler(Update update, ITelegramBotClient client)
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
                    if (await Settings(client, update, user))
                        return;
                    if (await Me(client, update, user))
                        return;
                    if (await ChooseVoice(client, update, user))
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
    }
}

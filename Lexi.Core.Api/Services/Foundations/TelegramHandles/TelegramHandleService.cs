//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Brokers.Speeches;
using Lexi.Core.Api.Brokers.Telegrams;
using Lexi.Core.Api.Brokers.UpdateStorages;
using Lexi.Core.Api.Models.Foundations.Users;
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
using Telegram.Bot.Types.Enums;

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
                Interval = TimeSpan.FromMinutes(10).TotalMilliseconds,
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

        //private Models.Foundations.Questions.Question ParseQuestionInfo(string line)
        //{
        //    // Split the line by commas
        //    string[] parts = line.Split(',');

        //    // Create a new Question instance
        //    Models.Foundations.Questions.Question question = new Models.Foundations.Questions.Question();

        //    // Iterate through each part to extract question information
        //    foreach (var part in parts)
        //    {
        //        // Split each part by colon to separate property name and value
        //        string[] keyValue = part.Trim().Split(':');

        //        if (keyValue.Length == 2)
        //        {
        //            // Trim the property name and value
        //            string propertyName = keyValue[0].Trim();
        //            string propertyValue = keyValue[1].Trim();

        //            // Assign the value to the corresponding property of the question object
        //            switch (propertyName)
        //            {
        //                case "ID":
        //                    if (Guid.TryParse(propertyValue, out Guid id))
        //                        question.Id = id;
        //                    break;
        //                case "Content":
        //                    question.Content = propertyValue;
        //                    break;
        //                case "Number":
        //                    if (int.TryParse(propertyValue, out int number))
        //                        question.Number = number;
        //                    break;
        //                case "QuestionType":
        //                    question.QuestionType = propertyValue;
        //                    break;
        //                    // Add more cases for other properties if needed
        //            }
        //        }
        //    }

        //    return question;
        //}


        private async Task ProcessMessageHandler(Update update, ITelegramBotClient client)
        {
            try
            {
                Models.Foundations.Users.User user = this.updateStorageBroker
                     .SelectAllUsers().FirstOrDefault(u => u.TelegramId == update.Message.Chat.Id);

                //if (update.Message.Type == MessageType.Document && update.Message.Document.FileName == "questions.txt")
                //{
                //    var fileId = update.Message.Document.FileId;

                //    // Download the file
                //    var file = await client.GetFileAsync(fileId);

                //    using (var fileStream = new MemoryStream())
                //    {
                //        // Download the file's content into a MemoryStream
                //        await client.DownloadFileAsync(file.FilePath, fileStream);

                //        // Reset the position of the stream to the beginning
                //        fileStream.Position = 0;

                //        using (var reader = new StreamReader(fileStream))
                //        {
                //            // Read the content of the file line by line
                //            string line;
                //            while ((line = reader.ReadLine()) != null)
                //            {
                //                // Parse the line to extract question information
                //                Models.Foundations.Questions.Question questionInfo = ParseQuestionInfo(line);

                //                // Check if the question already exists in the database
                //                var existingQuestion = this.updateStorageBroker
                //                    .SelectAllQuestions().FirstOrDefault(q => q.Id == questionInfo.Id);

                //                // If the question exists, update the question; otherwise, insert the question
                //                if (existingQuestion != null)
                //                {
                //                    await this.updateStorageBroker.UpdateQuestionAsync(questionInfo);
                //                }
                //                else
                //                {
                //                    await this.updateStorageBroker.InsertQuestionAsync(questionInfo);
                //                }
                //            }
                //        }
                //    }

                //    await client.SendTextMessageAsync(update.Message.Chat.Id, "Questions added to the database.");
                //}


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

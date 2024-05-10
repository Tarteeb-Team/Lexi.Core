//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lexi.Core.Api.Brokers.UpdateStorages;
using Lexi.Core.Api.Services.Foundations.ImproveSpeech;
using Lexi.Core.Api.Services.Orchestrations;
using Microsoft.AspNetCore.Hosting;
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
		private static readonly AsyncLocal<long> storedTelegramId = new AsyncLocal<long>();
		private static readonly AsyncLocal<int> messageId = new AsyncLocal<int>();
		private static readonly AsyncLocal<string> telegramName = new AsyncLocal<string>();
		private static readonly AsyncLocal<string> storedName = new AsyncLocal<string>();
		private static readonly AsyncLocal<string> storedLevel = new AsyncLocal<string>();
		private readonly System.Timers.Timer dailyNotificationTimer;
		private readonly System.Timers.Timer requestTimer;
		private DateTime lastNotificationTime;
		private string filePath;
		private string userPath;

		public TelegramBroker(
			IWebHostEnvironment hostingEnvironment,
			IOpenAIService openAIService,
			IUpdateStorageBroker updateStorageBroker)
		{
			var token = "6181821540:AAFACtaOd81KAZhqwvziu08S7z7FyIdeN_E";
			this.botClient = new TelegramBotClient(token);
			this._hostingEnvironment = hostingEnvironment;
			filePath = Path.Combine(this._hostingEnvironment.WebRootPath, "outputWavs/");
			userPath = null;
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
					if (await VoiceMessage(client, update, user))
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

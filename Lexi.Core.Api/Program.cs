//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using aisha_ai.Services.Foundations.HandleSpeeches;
using Lexi.Core.Api.Brokers.Cognitives;
using Lexi.Core.Api.Brokers.Loggings;
using Lexi.Core.Api.Brokers.OpenAIs;
using Lexi.Core.Api.Brokers.Speeches;
using Lexi.Core.Api.Brokers.Storages;
using Lexi.Core.Api.Brokers.TelegramBroker;
using Lexi.Core.Api.Services.Cognitives;
using Lexi.Core.Api.Services.Foundations.Feedbacks;
using Lexi.Core.Api.Services.Foundations.ImproveSpeech;
using Lexi.Core.Api.Services.Foundations.Speeches;
using Lexi.Core.Api.Services.Foundations.Telegrams;
using Lexi.Core.Api.Services.Foundations.Users;
using Lexi.Core.Api.Services.Orchestrations;
using Lexi.Core.Api.Services.Orchestrations.Cognitive;
using Lexi.Core.Api.Services.Orchestrations.Speech;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<IStorageBroker, StorageBroker>();
builder.Services.AddSingleton<IStorageBroker, StorageBroker>();
builder.Services.AddTransient<ILoggingBroker, LoggingBroker>();
builder.Services.AddTransient<ISpeechBroker, SpeechBroker>();
builder.Services.AddTransient<IHandleSpeechService, HandleSpeechService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IOpenAIService, OpenAIService>();
builder.Services.AddTransient<IOpenAIBroker, OpenAIBroker>();
builder.Services.AddTransient<ISpeechService, SpeechService>();
builder.Services.AddTransient<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IOrchestrationService, OrchestrationService>();
builder.Services.AddTransient<ICognitiveServices, CognitiveServices>();
builder.Services.AddTransient<ICognitiveBroker, CognitiveBroker>();
builder.Services.AddScoped<ICognitiveOrchestrationService, CognitiveOrchestrationService>();
builder.Services.AddTransient<ISpeechOrchestrationService, SpeechOrchestrationService>();
builder.Services.AddScoped<ITelegramBroker, TelegramBroker>();
builder.Services.AddScoped<ITelegramService, TelegramService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();

using (var scope = scopeFactory.CreateScope())
{
    var telegramService = scope.ServiceProvider.GetRequiredService<ITelegramBroker>();
    var orchestrationService = scope.ServiceProvider.GetRequiredService<IOrchestrationService>();

    telegramService.StartListening();

    telegramService.SetOrchestrationService(orchestrationService);

}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

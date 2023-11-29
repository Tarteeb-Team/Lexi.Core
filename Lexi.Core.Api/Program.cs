using Lexi.Core.Api.Brokers.Cognitives;
using Lexi.Core.Api.Brokers.Loggings;
using Lexi.Core.Api.Brokers.Storages;
using Lexi.Core.Api.Brokers.TelegramBroker;
using Lexi.Core.Api.Services.Cognitives;
using Lexi.Core.Api.Services.Foundations.Feedbacks;
using Lexi.Core.Api.Services.Foundations.Speeches;
using Lexi.Core.Api.Services.Orchestrations.Cognitive;
using Lexi.Core.Api.Services.Orchestrations.Speech;
using Lexi.Core.Api.Services.Orchestrations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Lexi.Core.Api.Services.Foundations.Telegrams;
using Lexi.Core.Api.Services.Foundations.Users;

var builder = WebApplication.CreateBuilder(args);

// Добавляем сервисы
builder.Services.AddControllers();
builder.Services.AddDbContext<IStorageBroker, StorageBroker>();
builder.Services.AddTransient<ILoggingBroker, LoggingBroker>();
builder.Services.AddTransient<IUserService, UserService>();
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
    var teelgramSErvice = scope.ServiceProvider.GetRequiredService <ITelegramService>();

    teelgramSErvice.StartListening();

    var orchestrationService = scope.ServiceProvider.GetRequiredService<IOrchestrationService>();

    await orchestrationService.GenerateSpeechFeedbackForUser();
}



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

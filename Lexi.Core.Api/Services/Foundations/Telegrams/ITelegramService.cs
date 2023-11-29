//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Models.Foundations.ExternalUsers;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Services.Foundations.Telegrams
{
    public interface ITelegramService
    {
        ValueTask<ExternalUser> GetExternalUserAsync();
        ValueTask MapFeedbackToStringAndSendMessage(long telegramId, Feedback feedback, string sentence);
        void StartListening();
    }
}
